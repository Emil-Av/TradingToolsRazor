### Postgre Add Migration (not working)
Add-Migration MigrationName -Context PostgreSqlDbContext -OutputDir MigrationsPostgreSQL

### SqlServer Update Database
Update-Database -Context SqlServerDbContext

### Postgre Update Database
Update-Database -Context PostgreSqlDbContext

### Change environment variable and update database: VPSProd, LocalProdProfile, DevProfile
$env:ASPNETCORE_ENVIRONMENT="VPSProd" 

### VPS Postgre DB
user: emil
password: Fragile1012!

### Use this to Add Migration
dotnet ef migrations add AddEspresso --context PostgreSqlDbContext --output-dir MigrationsPostgreSQL --project DataAccess