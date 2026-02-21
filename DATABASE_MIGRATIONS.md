### SqlServer Add Migration
Add-Migration MigrationName -Context SqlServerDbContext  -OutputDir MigrationsSqlServer

### Postgre Add Migration
Add-Migration MigrationName -Context PostgreSqlDbContext -OutputDir MigrationsPostgreSQL

### SqlServer Update Database
Update-Database -Context SqlServerDbContext

### Postgre Update Database
Update-Database -Context PostgreSqlDbContext