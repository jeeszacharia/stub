using CIAMstubAPI;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<PersonDb>(opt => opt.UseInMemoryDatabase("PersonData"));
//builder.Services.AddScoped<DbContext, >().AddDbContext<PersonDb>(opt => opt.UseInMemoryDatabase("PersonData"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddAuthentication().AddJwtBearer();

builder.Services.AddAuthorizationBuilder()
  .AddPolicy("admin_greetings", policy =>
        policy
            .RequireRole("admin")
            .RequireClaim("scope", "greetings_api"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var PersonDetails = app.MapGroup("/Person");

PersonDetails.MapGet("/", GetAllPerson);
//PersonDetails.MapGet("/terms", Getaterms);
PersonDetails.MapGet("/{clientNumber}", Getpersondata);
//PersonDetails.MapPost("/", CreatePerson).RequireAuthorization("admin_greetings");
PersonDetails.MapPost("/", CreatePerson);
PersonDetails.MapPut("/{clientNumber}", UpdatePersonData);
PersonDetails.MapDelete("/{clientNumber}", DeletePerson);

app.Run();

static async Task<IResult> GetAllPerson(PersonDb db)
{
    return TypedResults.Ok(await db.PersonData.ToArrayAsync());
}

//static async Task<IResult> Getaterms(PersonDb db)
//{
//    return TypedResults.Ok(await db.PersonData.Where(t => t.clientNumber).ToListAsync());
//}

static async Task<IResult> Getpersondata(int clientNumber, PersonDb db)
{
    return await db.PersonData.FindAsync(clientNumber)
        is PersonModel person
            ? TypedResults.Ok(person)
            : TypedResults.Conflict("We could not find your record!");
}

static async Task<IResult> CreatePerson(PersonModel person, PersonDb db)
{
    var clientNumber = person.clientNumber;
    var data = await db.PersonData.FindAsync(clientNumber);
    if (data is null || data.clientNumber!=clientNumber)
    {
        db.PersonData.Add(person);
        await db.SaveChangesAsync();
        return TypedResults.Created($"/PersonData/{person.clientNumber}", person);
    }
    else
    {
        return TypedResults.BadRequest("clientNumber already exist");

    }


}


static async Task<IResult> UpdatePersonData(int clientNumber, PersonModel updatePerson, PersonDb db)
{

    //var localerson = await db.PersonData.FindAsync(clientNumber);
    var person = await db.PersonData.Where(x => x.clientNumber == clientNumber).AsNoTracking().FirstOrDefaultAsync();

    if (person is null) return TypedResults.NotFound();
    
    db.PersonData.Attach(person);   
    db.Entry(updatePerson).Property(pd=>pd.clientNumber).IsModified = false;
    db.Entry(updatePerson).Property(pd => pd.contactType).IsModified = true;
    db.Entry(updatePerson).Property(pd => pd.phoneNumber).IsModified = true;
    db.Entry(updatePerson).Property(pd => pd.email).IsModified = true;
    db.Entry(updatePerson).Property(pd => pd.ciamObjectId).IsModified = true;
    db.Entry(updatePerson).Property(pd => pd.IsTermsAccepted).IsModified = true;
    db.Entry(updatePerson).Property(pd => pd.acceptedTermsVersion).IsModified = true;
    db.Entry(updatePerson).Property(pd => pd.currentTermsVersion).IsModified = true;
       
  await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static async Task<IResult> DeletePerson(int clientNumber, PersonDb db)
{
    if (await db.PersonData.FindAsync(clientNumber) is PersonModel nzbn)
    {
        db.PersonData.Remove(nzbn);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}