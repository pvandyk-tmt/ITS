del ..\..\1_iTicket\src\main\assets\databases\iTicket.db
SQLite3 "..\..\1_iTicket\src\main\assets\databases\iTicket.db" < "Tables.txt"
SQLite3 "..\..\1_iTicket\src\main\assets\databases\iTicket.db" < "..\..\1_iTicket\src\main\assets\databases\iTicket.db_upgrade_1-2.sql"
SQLite3 "..\..\1_iTicket\src\main\assets\databases\iTicket.db" < "PostDeployment.txt"

pause