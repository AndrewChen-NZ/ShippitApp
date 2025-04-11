using ShippitApp;
using Microsoft.EntityFrameworkCore;
using DijkstraAlgorithm.Graphing;
using DijkstraAlgorithm.Pathing;

    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddDbContext<AppDb>(opt => opt.UseInMemoryDatabase("LineHauls"));
    var app = builder.Build();

    var newGraph = new GraphBuilder();

    app.MapPost("/route", async (LineHaul lineHaul, AppDb db) =>
    {
        if (lineHaul.To is null)
        {
            return Results.BadRequest("'To' field is missing");
        }
        else if (lineHaul.From is null)
        {
            return Results.BadRequest("'From' field is missing");
        }
        else if (lineHaul.Travel_Time_Seconds <= 0)
        {
            return Results.BadRequest("Travel time not set");
        }

        // Reject if Line Haul already exists
        var result = await db.LineHauls.Where(f => f.From == lineHaul.From && f.To == lineHaul.To).ToListAsync();
        if (result is null || result.Count < 1)
        {
            db.LineHauls.Add(lineHaul);
            await db.SaveChangesAsync();

            //prep for calculating distance
            try
            {
                newGraph.AddNode(lineHaul.To);
            }
            catch (Exception e)
            {
                if (!(e.GetType().Name == "GraphBuilderException"))
                {
                    throw new Exception("There was an issue while adding the Line Haul");
                }
            }
            try
            {
                newGraph.AddNode(lineHaul.From);
            }
            catch (Exception e)
            {
                if (!(e.GetType().Name == "GraphBuilderException"))
                {
                    throw new Exception("There was an issue while adding the Line Haul");
                }
            }
            try
            {
                newGraph.AddLink(lineHaul.From, lineHaul.To, lineHaul.Travel_Time_Seconds);
            }
            catch (Exception e)
            {
                if (!(e.GetType().Name == "GraphBuilderException"))
                {
                    throw new Exception("There was an issue while adding the Line Haul");
                }
            }

            return Results.Created($"/route/{lineHaul.Id}", lineHaul);
        }
        else
        {
            return Results.BadRequest("Line Haul already exists.");
        }
    }
    );

    app.MapGet("/routefetch", async (AppDb db) =>
        await db.LineHauls.ToListAsync());


    app.MapGet("/route", async (String? from, String? to, AppDb db) =>
    {
        if (from is null || to is null)
        {
            return Results.BadRequest("Missing query parameters from/to");
        }
        var routes = await db.LineHauls.ToListAsync();
        var graph = newGraph.Build();
        var pathfinder = new PathFinder(graph);
        try
        {
            var path = pathfinder.FindShortestPath(
                graph.Nodes.Single(node => node.Id == from),
                graph.Nodes.Single(node => node.Id == to)
                );
            var json = new PathJson();
            json.Path = new string[path.Segments.Count + 1];
            json.Path[0] = path.Segments.ElementAt(0).Origin.Id;

            for (int i = 1; i <= path.Segments.Count; i++)
            {
                json.Path[i] = path.Segments.ElementAt(i - 1).Destination.Id;
            }
            json.Travel_Time_Total_Seconds = (int)path.Segments.Sum(s => s.Weight);

            return Results.Ok(json);
        }
        catch (Exception e)
        {
            if (e.GetType().Name == "GraphBuilderException")
            {
                throw new Exception("Could not find a route for the requested locations.");
            }
        }
        return Results.InternalServerError();
    });

    app.Run();

public partial class Program { }