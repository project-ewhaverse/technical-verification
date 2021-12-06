import sys
import logging
import pymysql
import dbinfo
import json

connection = pymysql.connect(host = dbinfo.db_host, port = dbinfo.db_port,
    user = dbinfo.db_username, passwd = dbinfo.db_password, db = dbinfo.db_name)

def lambda_handler(event, context):
    # event ['body'] in command=login&id=idInput&password=passwordInput&info=infoInput
    
    command = event['body'].split('command=')[1].split('&id=')[0]
    id = event['body'].split('&id=')[1].split('&password=')[0]
    password = event['body'].split('&password=')[1].split('&info=')[0]
    info = event['body'].split('&info=')[1]
    
    if command == 'login':
        return lambda_login(id, password)
    elif command == 'register':
        return lambda_register(id, password)
    elif command == 'save':
        return lambda_save(id, password, info)
    else:
        return {
            'statusCode': 400,
            'body': "Invaild command"
        }

def lambda_login(id, password):
    
    query = f"select * from TestTable where BINARY id = '{id}' and BINARY password = '{password}'"
    cursor = connection.cursor();
    cursor.execute(query)
    rows = cursor.fetchall()
    
    if len(rows) == 0:
        return {
            'statusCode': 400,
            'body': "Fail to login"
        }
    else:
        return {
            'statusCode': 200,
            'body': rows[0][2]
        }

def lambda_register(id, password):
    
    query = f"select * from TestTable where BINARY id = '{id}'"
    cursor = connection.cursor();
    cursor.execute(query)
    rows = cursor.fetchall()
    
    if len(rows) == 0:
        sql = "INSERT INTO TestTable (id, password) VALUES (%s, %s)"
        val = (id, password)
        cursor.execute(sql, val)
        connection.commit()
        return {
            'statusCode': 200,
            'body': "Register complete"
        }
    else:
        return {
            'statusCode': 400,
            'body': "Fail to register"
        }

def lambda_save(id, password, info):
    
    query = f"select * from TestTable where BINARY id = '{id}' and BINARY password = '{password}'"
    cursor = connection.cursor();
    cursor.execute(query)
    rows = cursor.fetchall()
    
    query = f"update TestTable set info = '{info}' where BINARY id = '{id}' and BINARY password = '{password}'"
    cursor = connection.cursor();
    cursor.execute(query)
    connection.commit()
    
    if len(rows) == 0:
        return {
            'statusCode': 400,
            'body': "Save fail"
        }
    else:
        return {
            'statusCode': 200,
            'body': "Save complete"
        }