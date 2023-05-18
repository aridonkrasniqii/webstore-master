using Microsoft.EntityFrameworkCore;
using WebStore.DAL;
using WebStore.Models;
namespace WebStore.Controllers;

public static class ReviewEndpointsClass
{
    public static void MapReviewEndpoints (this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/api/Review", async (WebStoreContext db) =>
        {
            return await db.Reviews.ToListAsync();
        })
        .WithName("GetAllReviews");

        routes.MapGet("/api/Review/{id}", async (int Id, WebStoreContext db) =>
        {
            return await db.Reviews.FindAsync(Id)
                is Review model
                    ? Results.Ok(model)
                    : Results.NotFound();
        })
        .WithName("GetReviewById");

        routes.MapPut("/api/Review/{id}", async (int Id, Review review, WebStoreContext db) =>
        {
            var foundModel = await db.Reviews.FindAsync(Id);

            if (foundModel is null)
            {
                return Results.NotFound();
            }

            db.Update(review);

            await db.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("UpdateReview");

        routes.MapPost("/api/Review/", async (Review review, WebStoreContext db) =>
        {
            db.Reviews.Add(review);
            await db.SaveChangesAsync();
            return Results.Created($"/Reviews/{review.Id}", review);
        })
        .WithName("CreateReview");

        routes.MapDelete("/api/Review/{id}", async (int Id, WebStoreContext db) =>
        {
            if (await db.Reviews.FindAsync(Id) is Review review)
            {
                db.Reviews.Remove(review);
                await db.SaveChangesAsync();
                return Results.Ok(review);
            }

            return Results.NotFound();
        })
        .WithName("DeleteReview");
    }
}
