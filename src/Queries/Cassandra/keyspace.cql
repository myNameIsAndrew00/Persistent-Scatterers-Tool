CREATE KEYSPACE UsersDataPoints
  WITH REPLICATION = { 
   'class' : 'SimpleStrategy', 
   'replication_factor' : 5
  };