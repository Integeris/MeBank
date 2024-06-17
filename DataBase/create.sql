--
-- PostgreSQL database dump
--

-- Dumped from database version 16.3
-- Dumped by pg_dump version 16.3

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: MeBank; Type: DATABASE; Schema: -; Owner: postgres
--

CREATE DATABASE "MeBank" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'Russian_Russia.1251';


ALTER DATABASE "MeBank" OWNER TO postgres;

\connect "MeBank"

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: system_stats; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS system_stats WITH SCHEMA public;


--
-- Name: EXTENSION system_stats; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION system_stats IS 'EnterpriseDB system statistics for PostgreSQL';


--
-- Name: BankAccountConversion(character varying, integer, integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public."BankAccountConversion"(IN "LoginParam" character varying, IN "idBankAccountParam" integer, IN "idNewCurrency" integer)
    LANGUAGE plpgsql
    AS $$
DECLARE
	"idClientVar" INT;
	"notExistBankAccount" BOOL;
	"notExistCurrency" BOOL;
	"debitCoefficient" NUMERIC;
	"creditCoefficient" NUMERIC;
	"multiplier" NUMERIC;
BEGIN
	-- Проверка на существование
	SELECT "Client"."IdClient"
	INTO "idClientVar"
	FROM "Client"
	WHERE "Client"."Login" = "LoginParam";
	
	SELECT COUNT(*) = 0
	INTO "notExistBankAccount"
	FROM "BankAccount"
	WHERE "BankAccount"."IdBankAccount" = "idBankAccountParam" AND
		"BankAccount"."IdClient" = "idClientVar";

	SELECT COUNT(*) = 0
	INTO "notExistCurrency"
	FROM "Currency"
	WHERE "Currency"."IdCurrency" = "idNewCurrency";

	IF "notExistBankAccount" THEN
		RAISE EXCEPTION 'There is no such thing as a bank account.'
		USING ERRCODE = '22023',
		DETAIL = 'The bank account does not exist in the database.',
		HINT = 'Select another bank account.';
	END IF;

	IF "notExistCurrency" THEN
		RAISE EXCEPTION 'Currency not found.'
		USING ERRCODE = '22023',
		DETAIL = 'Currency with this identifier is not found in the database.',
		HINT = 'Select another currency.';
	END IF;

	-- Просчёт коэффициентов

	SELECT "Currency"."ExchangeRate"
	INTO "debitCoefficient"
	FROM "BankAccount"
	INNER JOIN "Currency"
	ON "BankAccount"."IdCurrency" = "Currency"."IdCurrency"
	WHERE "BankAccount"."IdBankAccount" = "idBankAccountParam";

	SELECT "Currency"."ExchangeRate"
	INTO "creditCoefficient"
	FROM "Currency"
	WHERE "Currency"."IdCurrency" = "idNewCurrency";

	"multiplier" := "debitCoefficient" / "creditCoefficient";

	-- Конвертация баланса банковского счёта.

	UPDATE "BankAccount"
	SET "Balance" = "Balance" * "multiplier",
		"IdCurrency" = "idNewCurrency"
	WHERE "BankAccount"."IdBankAccount" = "idBankAccountParam";

	-- Конвертация истории банковского счёта.

	UPDATE "Entry"
	SET "DebitAmount" = "DebitAmount" * "multiplier"
	WHERE "Entry"."IdDebitAccount" = "idBankAccountParam";

	UPDATE "Entry"
	SET "CreditAmount" = "CreditAmount" * "multiplier"
	WHERE "Entry"."IdCreditAccount" = "idBankAccountParam";
END;
$$;


ALTER PROCEDURE public."BankAccountConversion"(IN "LoginParam" character varying, IN "idBankAccountParam" integer, IN "idNewCurrency" integer) OWNER TO postgres;

--
-- Name: CreateBankAccount(character varying, numeric, integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public."CreateBankAccount"("LoginParam" character varying, "BalanceParam" numeric DEFAULT 0, "IdCurrency" integer DEFAULT 1) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
	"idClientVar" INT;
	"bankAccountCount" INT;
BEGIN
	SELECT "Client"."IdClient"
	INTO "idClientVar"
	FROM "Client"
	WHERE "Client"."Login" = "LoginParam";

	SELECT COUNT(*)
	INTO "bankAccountCount"
	FROM "BankAccount"
	WHERE "BankAccount"."IdClient" = "idClientVar";

	IF "bankAccountCount" >= 5 THEN
		RAISE EXCEPTION 'You cannot create more than five bank accounts.'
		USING ERRCODE = '22023',
		DETAIL = 'You cannot create more than five bank accounts per user. Delete another account to create a new one.',
		HINT = 'Delete other accounts or use old accounts.';
	END IF;

	INSERT INTO "BankAccount" ("IdClient", "Balance", "IdCurrency")
	VALUES ("idClientVar", "BalanceParam", "IdCurrency")
	RETURNING "BankAccount"."IdBankAccount"
	INTO "idClientVar";

	RETURN "idClientVar";
END;
$$;


ALTER FUNCTION public."CreateBankAccount"("LoginParam" character varying, "BalanceParam" numeric, "IdCurrency" integer) OWNER TO postgres;

--
-- Name: CreateClient(character varying, character varying); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public."CreateClient"(IN "LoginParam" character varying, IN "PasswordParam" character varying)
    LANGUAGE plpgsql
    AS $$
DECLARE 
	"loginLength" INT;
	"passwordLength" INT;
	"idNewClient" INT;
BEGIN
	"loginLength" := LENGTH("LoginParam");
	"passwordLength" := LENGTH("PasswordParam");

	SELECT COUNT(*)
	INTO "idNewClient"
	FROM "ExistClient"("LoginParam");

	IF "loginLength" < 5 OR "loginLength" > 50 THEN
		RAISE EXCEPTION 'Invalid login.'
		USING ERRCODE = '22023',
		DETAIL = 'Invalid login. Login must consist of letters or numbers from 5 to 50.',
		HINT = 'Enter another login.';
	END IF;

	IF "passwordLength" < 5 OR "passwordLength" > 50 THEN
		RAISE EXCEPTION 'Invalid password.'
		USING ERRCODE = '22023',
		DETAIL = 'Invalid password. password must consist of letters or numbers from 5 to 50.',
		HINT = 'Enter another password.';
	END IF;

	INSERT INTO "Client"("Login", "Password")
	VALUES("LoginParam", "PasswordParam")
	RETURNING "Client"."IdClient"
	INTO "idNewClient";

	SELECT COUNT(*)
	INTO "loginLength"
	FROM "CreateBankAccount"("LoginParam", 100, 1);
END;
$$;


ALTER PROCEDURE public."CreateClient"(IN "LoginParam" character varying, IN "PasswordParam" character varying) OWNER TO postgres;

--
-- Name: ExistClient(character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public."ExistClient"("LoginParam" character varying) RETURNS boolean
    LANGUAGE plpgsql
    AS $$
DECLARE 
	"clientExist" BOOL;
BEGIN
	SELECT COUNT(*) > 0
	INTO "clientExist"
	FROM "Client"
	WHERE "Client"."Login" = "LoginParam";
	RETURN "clientExist";
END;
$$;


ALTER FUNCTION public."ExistClient"("LoginParam" character varying) OWNER TO postgres;

--
-- Name: ExistClient(character varying, character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public."ExistClient"("LoginParam" character varying, "PasswordParam" character varying) RETURNS boolean
    LANGUAGE plpgsql
    AS $$
DECLARE 
	"clientExist" BOOL;
BEGIN
	SELECT COUNT(*) > 0
	INTO "clientExist"
	FROM "Client"
	WHERE "Client"."Login" = "LoginParam" AND 
		"Client"."Password" = "PasswordParam";
	RETURN "clientExist";
END;
$$;


ALTER FUNCTION public."ExistClient"("LoginParam" character varying, "PasswordParam" character varying) OWNER TO postgres;

--
-- Name: GetBankAccountEntries(integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public."GetBankAccountEntries"("IdBankAccountParam" integer) RETURNS TABLE("IdEntry" integer, "IdBankAccount" integer, "Amount" numeric, "DATE" timestamp without time zone)
    LANGUAGE plpgsql
    AS $$
DECLARE
	"idBankAccountNotExist" BOOL;
BEGIN
	SELECT COUNT(*) = 0
	INTO "idBankAccountNotExist"
	FROM "BankAccount"
	WHERE "BankAccount"."IdBankAccount" = "IdBankAccountParam";

	IF "idBankAccountNotExist" THEN
		RAISE EXCEPTION 'The bank account does not exist.'
		USING DETAIL = 'The bank account is not created. No data about him is stored in the database.',
		HINT = 'Create the bank account or set another bank account.',
		ERRCODE = '22023';
	END IF;

	RETURN QUERY
	SELECT "Entry"."IdEntry",
		CASE WHEN "Entry"."IdCreditAccount" = "IdBankAccountParam"
		 	THEN "Entry"."IdDebitAccount"
			ELSE "Entry"."IdCreditAccount"
		END AS "IdBankAccount",
		CASE WHEN "Entry"."IdCreditAccount" = "IdBankAccountParam"
		 	THEN "Entry"."CreditAmount"
			ELSE -"Entry"."DebitAmount"
		END AS "Amount",
		"Entry"."Date"
	FROM "BankAccount"
	INNER JOIN "Entry"
	ON "BankAccount"."IdBankAccount" = "Entry"."IdCreditAccount" OR
		"BankAccount"."IdBankAccount" = "Entry"."IdDebitAccount"
	WHERE "BankAccount"."IdBankAccount" = "IdBankAccountParam"
	ORDER BY "Entry"."Date" DESC;
END;
$$;


ALTER FUNCTION public."GetBankAccountEntries"("IdBankAccountParam" integer) OWNER TO postgres;

--
-- Name: GetCurrencies(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public."GetCurrencies"() RETURNS TABLE("IdCurrency" integer, "Title" character varying, "ExchangeRate" numeric)
    LANGUAGE plpgsql
    AS $$
BEGIN
	RETURN QUERY
	SELECT "Currency"."IdCurrency",
		"Currency"."Title",
		"Currency"."ExchangeRate"
	FROM "Currency";
END;
$$;


ALTER FUNCTION public."GetCurrencies"() OWNER TO postgres;

--
-- Name: GetUserBankAccounts(integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public."GetUserBankAccounts"("ClientId" integer) RETURNS TABLE("IdClient" integer, "IdBankAccount" integer, "Balance" numeric, "CurrencyTitle" character varying)
    LANGUAGE plpgsql
    AS $$
DECLARE
	userNotExist BOOL;
BEGIN
	SELECT COUNT(*) = 0
	INTO userNotExist
	FROM "Client"
	WHERE "Client"."IdClient" = "ClientId";

	IF userNotExist THEN
		RAISE EXCEPTION 'The user does not exist.'
		USING DETAIL = 'The user is not registered. No data about him is stored in the database.',
		HINT = 'Register the user or log in as another user.',
		ERRCODE = '22023';
	END IF;

	RETURN QUERY
	SELECT "BankAccount"."IdClient", 
		"BankAccount"."IdBankAccount",
		"BankAccount"."Balance",
		"Currency"."Title"
	FROM "BankAccount"
	INNER JOIN "Currency"
	ON "BankAccount"."IdCurrency" = "Currency"."IdCurrency"
	WHERE "BankAccount"."IdClient" = "ClientId";
END;
$$;


ALTER FUNCTION public."GetUserBankAccounts"("ClientId" integer) OWNER TO postgres;

--
-- Name: GetUserBankAccounts(character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public."GetUserBankAccounts"("ClientLogin" character varying) RETURNS TABLE("IdClient" integer, "IdBankAccount" integer, "Balance" numeric, "CurrencyTitle" character varying)
    LANGUAGE plpgsql
    AS $$
DECLARE
	userNotExist BOOL;
BEGIN
	SELECT COUNT(*) = 0
	INTO userNotExist
	FROM "Client"
	WHERE "Client"."Login" = "ClientLogin";

	IF userNotExist THEN
		RAISE EXCEPTION 'The user does not exist.'
		USING DETAIL = 'The user is not registered. No data about him is stored in the database.',
		HINT = 'Register the user or log in as another user.',
		ERRCODE = '22023';
	END IF;

	RETURN QUERY
	SELECT "BankAccount"."IdClient", 
		"BankAccount"."IdBankAccount",
		"BankAccount"."Balance",
		"Currency"."Title"
	FROM "BankAccount"
	INNER JOIN "Currency"
	ON "BankAccount"."IdCurrency" = "Currency"."IdCurrency"
	INNER JOIN "Client"
	ON "BankAccount"."IdClient" = "Client"."IdClient"
	WHERE "Client"."Login" = "ClientLogin"
	ORDER BY "BankAccount"."IdBankAccount" ASC;
END;
$$;


ALTER FUNCTION public."GetUserBankAccounts"("ClientLogin" character varying) OWNER TO postgres;

--
-- Name: MoneyTransfer(character varying, integer, integer, numeric); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public."MoneyTransfer"(IN "LoginParam" character varying, IN "IdDebitAccount" integer, IN "IdCreditAccount" integer, IN "Amount" numeric)
    LANGUAGE plpgsql
    AS $$
DECLARE 
	"notBelongClient" BOOl;
	debitNotExist BOOL;
	creditNotExist BOOL;
	notEnoughMoney BOOL;
	"debitCoefficient" NUMERIC;
	"creditCoefficient" NUMERIC;
	"multiplier" NUMERIC;
BEGIN
	SELECT COUNT(*) = 0
	FROM "BankAccount"
	INTO "notBelongClient"
	INNER JOIN "Client"
	ON "BankAccount"."IdClient" = "Client"."IdClient"
	WHERE "Client"."Login" = "LoginParam" AND
		("BankAccount"."IdBankAccount" = "IdDebitAccount" OR
		"BankAccount"."IdBankAccount" = "IdCreditAccount");

	IF "notBelongClient" THEN
		RAISE EXCEPTION 'No account is owned by the client.'
		USING ERRCODE = '22023',
		DETAIL = 'Neither the debit account nor the credit account belongs to the client.',
		HINT = 'Specify another account.';
	END IF;

	IF "Amount" <= 0 THEN
		RAISE EXCEPTION 'Money cannot be negative.'
		USING ERRCODE = '22023',
		DETAIL = 'Money cannot be negative. They must be greater than 0.',
		HINT = 'Specify a different transfer amount.';
	END IF;

	SELECT (COUNT(*) = 0)::BOOL
	INTO debitNotExist
	FROM "BankAccount"
	WHERE "BankAccount"."IdBankAccount" = "IdDebitAccount";

	SELECT (COUNT(*) = 0)::BOOL
	INTO creditNotExist
	FROM "BankAccount"
	WHERE "BankAccount"."IdBankAccount" = "IdCreditAccount";

	SELECT ("BankAccount"."Balance" < "Amount")::BOOL
	INTO notEnoughMoney
	FROM "BankAccount"
	WHERE "BankAccount"."IdBankAccount" = "IdDebitAccount";

	-- Вывод исключений

	IF "Amount" <= 0 THEN
		RAISE EXCEPTION 'The transfer amount cannot be less than or equal to zero.'
		USING ERRCODE = '22023',
		DETAIL = 'The transfer amount cannot be less than or equal to zero. It can only be positive.',
		HINT = 'Enter a different transfer amount.';
	END IF;
		
	IF debitNotExist THEN
		RAISE EXCEPTION 'The debit account does not exist.'
		USING ERRCODE = '22023',
		DETAIL = 'The debit account does not exist. Unable to transfer funds from a non-existent account.',
		HINT = 'Specify another account.';
	END IF;

	IF creditNotExist THEN
		RAISE EXCEPTION 'There is no receipts account.'
		USING ERRCODE = '22023',
		DETAIL = 'There is no receipts account. Unable to transfer funds to an account that does not exist.',
		HINT = 'Specify another account.';
	END IF;

	IF notEnoughMoney THEN
		RAISE EXCEPTION 'Not enough money.'
		USING ERRCODE = '22023',
		DETAIL = 'Not enough money to transfer to the user.',
		HINT = 'Refill your balance.';
	END IF;

	-- Просчёт коэффицентов

	SELECT "Currency"."ExchangeRate"
	INTO "debitCoefficient"
	FROM "BankAccount"
	INNER JOIN "Currency"
	ON "BankAccount"."IdCurrency" = "Currency"."IdCurrency"
	WHERE "BankAccount"."IdBankAccount" = "IdDebitAccount";

	SELECT "Currency"."ExchangeRate"
	INTO "creditCoefficient"
	FROM "BankAccount"
	INNER JOIN "Currency"
	ON "BankAccount"."IdCurrency" = "Currency"."IdCurrency"
	WHERE "BankAccount"."IdBankAccount" = "IdCreditAccount";

	multiplier := "debitCoefficient" / "creditCoefficient";

	-- Обновление данных

	UPDATE "BankAccount"
	SET "Balance" = "Balance" - "Amount"
	WHERE "BankAccount"."IdBankAccount" = "IdDebitAccount";
	
	UPDATE "BankAccount"
	SET "Balance" = "Balance" + "Amount" * "multiplier"
	WHERE "BankAccount"."IdBankAccount" = "IdCreditAccount";
	
	INSERT INTO "Entry"("IdDebitAccount", "IdCreditAccount", "DebitAmount", "CreditAmount", "Date")
	VALUES ("IdDebitAccount", "IdCreditAccount", "Amount", "Amount" * "multiplier", NOW());
END;
$$;


ALTER PROCEDURE public."MoneyTransfer"(IN "LoginParam" character varying, IN "IdDebitAccount" integer, IN "IdCreditAccount" integer, IN "Amount" numeric) OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: BankAccount; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."BankAccount" (
    "IdBankAccount" integer NOT NULL,
    "IdClient" integer NOT NULL,
    "Balance" numeric(1000,6) DEFAULT 0 NOT NULL,
    "IdCurrency" integer NOT NULL
);


ALTER TABLE public."BankAccount" OWNER TO postgres;

--
-- Name: BankAccount_IdBankAccount_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."BankAccount_IdBankAccount_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."BankAccount_IdBankAccount_seq" OWNER TO postgres;

--
-- Name: BankAccount_IdBankAccount_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."BankAccount_IdBankAccount_seq" OWNED BY public."BankAccount"."IdBankAccount";


--
-- Name: Client; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Client" (
    "IdClient" integer NOT NULL,
    "Login" character varying(50) NOT NULL,
    "Password" character varying(50) NOT NULL
);


ALTER TABLE public."Client" OWNER TO postgres;

--
-- Name: Client_IdClient_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Client_IdClient_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Client_IdClient_seq" OWNER TO postgres;

--
-- Name: Client_IdClient_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Client_IdClient_seq" OWNED BY public."Client"."IdClient";


--
-- Name: Currency; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Currency" (
    "IdCurrency" integer NOT NULL,
    "Title" character varying(50) NOT NULL,
    "ExchangeRate" numeric NOT NULL
);


ALTER TABLE public."Currency" OWNER TO postgres;

--
-- Name: Currency_IdCurrency_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Currency_IdCurrency_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Currency_IdCurrency_seq" OWNER TO postgres;

--
-- Name: Currency_IdCurrency_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Currency_IdCurrency_seq" OWNED BY public."Currency"."IdCurrency";


--
-- Name: Entry; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Entry" (
    "IdEntry" integer NOT NULL,
    "IdDebitAccount" integer NOT NULL,
    "IdCreditAccount" integer NOT NULL,
    "DebitAmount" numeric(1000,6) NOT NULL,
    "Date" timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "CreditAmount" numeric(1000,6) NOT NULL
);


ALTER TABLE public."Entry" OWNER TO postgres;

--
-- Name: Entry_IdEntry_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Entry_IdEntry_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Entry_IdEntry_seq" OWNER TO postgres;

--
-- Name: Entry_IdEntry_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Entry_IdEntry_seq" OWNED BY public."Entry"."IdEntry";


--
-- Name: BankAccount IdBankAccount; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BankAccount" ALTER COLUMN "IdBankAccount" SET DEFAULT nextval('public."BankAccount_IdBankAccount_seq"'::regclass);


--
-- Name: Client IdClient; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Client" ALTER COLUMN "IdClient" SET DEFAULT nextval('public."Client_IdClient_seq"'::regclass);


--
-- Name: Currency IdCurrency; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Currency" ALTER COLUMN "IdCurrency" SET DEFAULT nextval('public."Currency_IdCurrency_seq"'::regclass);


--
-- Name: Entry IdEntry; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Entry" ALTER COLUMN "IdEntry" SET DEFAULT nextval('public."Entry_IdEntry_seq"'::regclass);


--
-- Data for Name: BankAccount; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."BankAccount" ("IdBankAccount", "IdClient", "Balance", "IdCurrency") FROM stdin;
3	1	2.000000	2
2	1	40.000000	1
4	1	0.000000	1
5	1	0.000000	2
1	1	20.000000	1
\.


--
-- Data for Name: Client; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Client" ("IdClient", "Login", "Password") FROM stdin;
1	admin	admin
\.


--
-- Data for Name: Currency; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Currency" ("IdCurrency", "Title", "ExchangeRate") FROM stdin;
1	Ruble	1
2	CU	20
\.


--
-- Data for Name: Entry; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Entry" ("IdEntry", "IdDebitAccount", "IdCreditAccount", "DebitAmount", "Date", "CreditAmount") FROM stdin;
8	2	3	20.000000	2024-06-16 23:07:04.281352	1.000000
12	2	3	10.000000	2024-06-17 20:42:22.389839	0.500000
1	1	2	20.000000	2024-06-16 17:09:17.76663	20.000000
2	1	2	10.000000	2024-06-16 17:10:41.515197	10.000000
6	1	2	30.000000	2024-06-16 17:28:39.138455	30.000000
4	1	3	20.000000	2024-06-16 17:28:08.739535	1.000000
7	1	3	20.000000	2024-06-16 23:06:49.522323	1.000000
11	1	2	30.000000	2024-06-17 20:41:07.191911	30.000000
13	1	2	10.000000	2024-06-17 20:44:46.645501	10.000000
3	2	1	10.000000	2024-06-16 17:11:08.745971	10.000000
9	2	1	20.000000	2024-06-16 23:13:42.075688	20.000000
5	3	1	0.500000	2024-06-16 17:28:26.946809	10.000000
10	3	1	1.000000	2024-06-17 20:40:28.932852	20.000000
\.


--
-- Name: BankAccount_IdBankAccount_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."BankAccount_IdBankAccount_seq"', 5, true);


--
-- Name: Client_IdClient_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Client_IdClient_seq"', 2, true);


--
-- Name: Currency_IdCurrency_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Currency_IdCurrency_seq"', 2, true);


--
-- Name: Entry_IdEntry_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Entry_IdEntry_seq"', 13, true);


--
-- Name: BankAccount BankAccount_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BankAccount"
    ADD CONSTRAINT "BankAccount_pkey" PRIMARY KEY ("IdBankAccount");


--
-- Name: Client Client_Login_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Client"
    ADD CONSTRAINT "Client_Login_key" UNIQUE ("Login");


--
-- Name: Client Client_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Client"
    ADD CONSTRAINT "Client_pkey" PRIMARY KEY ("IdClient");


--
-- Name: Currency Currency_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Currency"
    ADD CONSTRAINT "Currency_pkey" PRIMARY KEY ("IdCurrency");


--
-- Name: Entry Entry_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Entry"
    ADD CONSTRAINT "Entry_pkey" PRIMARY KEY ("IdEntry");


--
-- Name: BankAccount BankAccount_IdClient_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BankAccount"
    ADD CONSTRAINT "BankAccount_IdClient_fkey" FOREIGN KEY ("IdClient") REFERENCES public."Client"("IdClient") ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: BankAccount BankAccount_IdCurrency_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BankAccount"
    ADD CONSTRAINT "BankAccount_IdCurrency_fkey" FOREIGN KEY ("IdCurrency") REFERENCES public."Currency"("IdCurrency") ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: Entry Entry_CreditAccount_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Entry"
    ADD CONSTRAINT "Entry_CreditAccount_fkey" FOREIGN KEY ("IdCreditAccount") REFERENCES public."BankAccount"("IdBankAccount") ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: Entry Entry_DebitAccount_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Entry"
    ADD CONSTRAINT "Entry_DebitAccount_fkey" FOREIGN KEY ("IdDebitAccount") REFERENCES public."BankAccount"("IdBankAccount") ON UPDATE CASCADE ON DELETE CASCADE;


-- Role: "Server"
-- DROP ROLE IF EXISTS "Server";

CREATE ROLE "Server" WITH
  LOGIN
  NOSUPERUSER
  INHERIT
  NOCREATEDB
  NOCREATEROLE
  NOREPLICATION
  NOBYPASSRLS
  ENCRYPTED PASSWORD 'SCRAM-SHA-256$4096:GUflWZUKKHWcf7kMvXlizA==$Ryr5HP9RuHQWg7sAdgT4GdZHT1hEw9sbS5+YVR5Lj30=:PgCXT02gPYoIQzFdVDrbneaKkoiCE/0jFJO7OiPqgGk=';

--
-- Name: DATABASE "MeBank"; Type: ACL; Schema: -; Owner: postgres
--

GRANT CONNECT ON DATABASE "MeBank" TO "Server";


--
-- Name: FUNCTION "ExistClient"("LoginParam" character varying); Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON FUNCTION public."ExistClient"("LoginParam" character varying) TO "Server";


--
-- Name: FUNCTION "GetBankAccountEntries"("IdBankAccountParam" integer); Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON FUNCTION public."GetBankAccountEntries"("IdBankAccountParam" integer) TO "Server";


--
-- Name: FUNCTION "GetCurrencies"(); Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON FUNCTION public."GetCurrencies"() TO "Server";


--
-- Name: FUNCTION "GetUserBankAccounts"("ClientId" integer); Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON FUNCTION public."GetUserBankAccounts"("ClientId" integer) TO "Server";


--
-- Name: FUNCTION "GetUserBankAccounts"("ClientLogin" character varying); Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON FUNCTION public."GetUserBankAccounts"("ClientLogin" character varying) TO "Server";


--
-- Name: PROCEDURE "MoneyTransfer"(IN "LoginParam" character varying, IN "IdDebitAccount" integer, IN "IdCreditAccount" integer, IN "Amount" numeric); Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON PROCEDURE public."MoneyTransfer"(IN "LoginParam" character varying, IN "IdDebitAccount" integer, IN "IdCreditAccount" integer, IN "Amount" numeric) TO "Server";


--
-- Name: TABLE "BankAccount"; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public."BankAccount" TO "Server";


--
-- Name: SEQUENCE "BankAccount_IdBankAccount_seq"; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public."BankAccount_IdBankAccount_seq" TO "Server";


--
-- Name: TABLE "Client"; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public."Client" TO "Server" WITH GRANT OPTION;


--
-- Name: SEQUENCE "Client_IdClient_seq"; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public."Client_IdClient_seq" TO "Server";


--
-- Name: TABLE "Currency"; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public."Currency" TO "Server";


--
-- Name: SEQUENCE "Currency_IdCurrency_seq"; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public."Currency_IdCurrency_seq" TO "Server";


--
-- Name: TABLE "Entry"; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public."Entry" TO "Server";


--
-- Name: SEQUENCE "Entry_IdEntry_seq"; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public."Entry_IdEntry_seq" TO "Server";


--
-- PostgreSQL database dump complete
--

