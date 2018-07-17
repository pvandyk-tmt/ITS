del ..\..\console\src\main\assets\databases\Console.db
SQLite3 "..\..\console\src\main\assets\databases\Console.db" < "Tables.txt"
SQLite3 "..\..\console\src\main\assets\databases\Console.db" < "..\..\console\src\main\assets\databases\Console.db_upgrade_1-2.sql"
SQLite3 "..\..\console\src\main\assets\databases\Console.db" < "PostDeployment.txt"

pause
