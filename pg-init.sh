#!/bin/bash
set -euxo pipefail

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
  CREATE TABLE test_table (
      id integer PRIMARY KEY
  );
  INSERT INTO test_table (id) VALUES (1);
  INSERT INTO test_table (id) VALUES (2);
  INSERT INTO test_table (id) VALUES (3);
  INSERT INTO test_table (id) VALUES (4);
  INSERT INTO test_table (id) VALUES (5);
  
  CREATE PUBLICATION test_publication
  FOR TABLE test_table;
EOSQL