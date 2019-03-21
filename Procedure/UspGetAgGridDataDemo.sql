CREATE OR REPLACE PROCEDURE UspGetAgGridData (
    IN_FILTERCLAUSE IN CLOB DEFAULT NULL,
    IN_GROUP IN VARCHAR2,
    IN_WHERE IN VARCHAR2,
    IN_ORDERBY VARCHAR2 DEFAULT NULL,
    IN_PAGEINDEX IN NUMBER,
    IN_PAGESIZE IN NUMBER,
    OUT_COUNT OUT NUMBER,
    OUT_CURSOR OUT SYS_REFCURSOR)
AS 
  V_SQL CLOB;
  V_SQL_COUNT CLOB;
  V_FILTERCLAUSE CLOB;
BEGIN

  IF (IN_FILTERCLAUSE IS NULL) THEN
    V_FILTERCLAUSE:=' 1=1  ';
  ELSE
    V_FILTERCLAUSE:=IN_FILTERCLAUSE;
  END IF;

  V_SQL:='SELECT * FROM table where ' || IN_FILTERCLAUSE || IN_ORDERBY ;

  IF (IN_GROUP IS NOT NULL) THEN
    V_SQL:='SELECT COUNT(*) AS COUNT, '||IN_GROUP||' FROM (' || V_SQL ||' )gop GROUP BY '||IN_GROUP||' ORDER BY '||
    IN_GROUP||'' ;
  END IF;
  IF (IN_WHERE IS NOT NULL) THEN
    V_SQL:='SELECT *  FROM (' || V_SQL ||' )WHE  where 1=1 '||IN_WHERE||'' ;
  END IF;
  V_SQL_COUNT:='SELECT COUNT(1) FROM (' || V_SQL ||' ) CO';
  DBMS_OUTPUT.PUT_LINE (V_SQL_COUNT) ;
  EXECUTE IMMEDIATE V_SQL_COUNT INTO OUT_COUNT;
  
  --OUT_COUNT:=90000;
  IF (IN_GROUP IS NOT NULL) THEN
    V_SQL:='SELECT R.* FROM ( SELECT VSQL.*,ROWNUM RN  FROM ('||V_SQL||')VSQL WHERE ROWNUM<='||IN_PAGESIZE*IN_PAGEINDEX
    ||' )R  WHERE RN>'||IN_PAGESIZE* (IN_PAGEINDEX-1) ;
  ELSE
    V_SQL:='SELECT * FROM ('||V_SQL||') WHERE ROWID IN ( SELECT RID FROM ( SELECT VSQL.*,ROWNUM RN,ROWID RID  FROM ('||
    V_SQL||')VSQL WHERE ROWNUM<='||IN_PAGESIZE*IN_PAGEINDEX||' )R  WHERE RN>'||IN_PAGESIZE* (IN_PAGEINDEX-1) ||')';
  END IF;
  DBMS_OUTPUT.PUT_LINE (V_SQL) ;
  OPEN OUT_CURSOR FOR V_SQL;
END ;