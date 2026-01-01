# Backend API Implementation Required

## Add these endpoints to MasterController.cs:

```csharp
[HttpGet("GetCountries")]
public IActionResult GetCountries()
{
    try
    {
        var countries = _context.Countries
            .Where(c => c.IsActive)
            .OrderBy(c => c.CountryName)
            .Select(c => new
            {
                countryCode = c.CountryCode,
                countryName = c.CountryName,
                isActive = c.IsActive
            })
            .ToList();
        
        return Ok(countries);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { message = ex.Message });
    }
}

[HttpGet("GetStatesByCountry")]
public IActionResult GetStatesByCountry(string countryCode)
{
    try
    {
        var states = _context.States
            .Where(s => s.CountryCode == countryCode && s.IsActive)
            .OrderBy(s => s.StateName)
            .Select(s => new
            {
                stateCode = s.StateCode,
                stateName = s.StateName,
                countryCode = s.CountryCode,
                isActive = s.IsActive
            })
            .ToList();
        
        return Ok(states);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { message = ex.Message });
    }
}
```

## Add these models to Modal folder:

### CountryDTO.cs
```csharp
public class CountryDTO
{
    public string CountryCode { get; set; }
    public string CountryName { get; set; }
    public bool IsActive { get; set; }
}
```

### StateDTO.cs
```csharp
public class StateDTO
{
    public string StateCode { get; set; }
    public string StateName { get; set; }
    public string CountryCode { get; set; }
    public bool IsActive { get; set; }
}
```

## Add DbSet properties to DbContext:

```csharp
public DbSet<CountryDTO> Countries { get; set; }
public DbSet<StateDTO> States { get; set; }
```

## Configure entity mappings in OnModelCreating:

```csharp
modelBuilder.Entity<CountryDTO>(entity =>
{
    entity.ToTable("tbl_country");
    entity.HasKey(e => e.CountryCode);
    entity.Property(e => e.CountryCode).HasColumnName("country_code");
    entity.Property(e => e.CountryName).HasColumnName("country_name");
    entity.Property(e => e.IsActive).HasColumnName("is_active");
});

modelBuilder.Entity<StateDTO>(entity =>
{
    entity.ToTable("tbl_state");
    entity.HasKey(e => e.StateCode);
    entity.Property(e => e.StateCode).HasColumnName("state_code");
    entity.Property(e => e.StateName).HasColumnName("state_name");
    entity.Property(e => e.CountryCode).HasColumnName("country_code");
    entity.Property(e => e.IsActive).HasColumnName("is_active");
});
```
