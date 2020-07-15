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

### Supported Applications
- Excel
- Firebird
- MSSQL
- MySql
- Postgre

### Schema Sub-Commands

#### Get tables
```
-d excel -u "" -p "" -i "" -c "C:\path\to\folder\file.xlsx" -s "schema:tables" -o json
```
#### Get columns
```
-d excel -u "" -p "" -i "" -c "C:\path\to\folder\file.xlsx" -s "schema:columns" -o json
```
#### Filter schema using Linq .Select()
```
-d excel -u "" -p "" -i "" -c "C:\path\to\folder\file.xlsx" -s "schema:columns" -f "COLUMN_NAME = 'city'" -o json
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

## Example Usage

### Excel
pass in an empty string for username, password and host.  The -c switch should be the full path to the excel file.
```
SqlClient.exe -d excel -u "" -p "" -i "" -c "C:\path\to\folder\file.xlsx" -s "select * from [clients$]" -o json
```

### Postgre
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