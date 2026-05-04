### SqlServer Add Migration
Add-Migration MigrationName -Context SqlServerDbContext  -OutputDir MigrationsSqlServer

### Postgre Add Migration
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