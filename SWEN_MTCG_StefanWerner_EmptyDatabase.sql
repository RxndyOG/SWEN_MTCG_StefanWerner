--
-- PostgreSQL database dump
--

-- Dumped from database version 17.0 (Debian 17.0-1.pgdg120+1)
-- Dumped by pg_dump version 17.2

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: cards; Type: TABLE; Schema: public; Owner: RxndyOG
--

CREATE TABLE public.cards (
    id integer NOT NULL,
    card_id character varying(200),
    "user" character varying(50),
    card_name character varying(100),
    value integer,
    damage numeric,
    family character varying(100),
    cardtype integer,
    element character varying(100)
);


ALTER TABLE public.cards OWNER TO "RxndyOG";

--
-- Name: cards_id_seq; Type: SEQUENCE; Schema: public; Owner: RxndyOG
--

CREATE SEQUENCE public.cards_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.cards_id_seq OWNER TO "RxndyOG";

--
-- Name: cards_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: RxndyOG
--

ALTER SEQUENCE public.cards_id_seq OWNED BY public.cards.id;


--
-- Name: deck; Type: TABLE; Schema: public; Owner: RxndyOG
--

CREATE TABLE public.deck (
    id character varying,
    username character varying,
    cardid1 character varying,
    cardid2 character varying,
    cardid3 character varying,
    cardid4 character varying
);


ALTER TABLE public.deck OWNER TO "RxndyOG";

--
-- Name: trading; Type: TABLE; Schema: public; Owner: RxndyOG
--

CREATE TABLE public.trading (
    username character varying,
    cardid character varying,
    type integer,
    damage numeric,
    id character varying
);


ALTER TABLE public.trading OWNER TO "RxndyOG";

--
-- Name: users; Type: TABLE; Schema: public; Owner: RxndyOG
--

CREATE TABLE public.users (
    id integer NOT NULL,
    username character varying(50) NOT NULL,
    password character varying(100) NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    image character varying(100),
    token character varying(100),
    coins integer,
    wins integer,
    lose integer,
    bio character varying(100)
);


ALTER TABLE public.users OWNER TO "RxndyOG";

--
-- Name: users_id_seq; Type: SEQUENCE; Schema: public; Owner: RxndyOG
--

CREATE SEQUENCE public.users_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.users_id_seq OWNER TO "RxndyOG";

--
-- Name: users_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: RxndyOG
--

ALTER SEQUENCE public.users_id_seq OWNED BY public.users.id;


--
-- Name: cards id; Type: DEFAULT; Schema: public; Owner: RxndyOG
--

ALTER TABLE ONLY public.cards ALTER COLUMN id SET DEFAULT nextval('public.cards_id_seq'::regclass);


--
-- Name: users id; Type: DEFAULT; Schema: public; Owner: RxndyOG
--

ALTER TABLE ONLY public.users ALTER COLUMN id SET DEFAULT nextval('public.users_id_seq'::regclass);


--
-- Data for Name: cards; Type: TABLE DATA; Schema: public; Owner: RxndyOG
--

COPY public.cards (id, card_id, "user", card_name, value, damage, family, cardtype, element) FROM stdin;
\.


--
-- Data for Name: deck; Type: TABLE DATA; Schema: public; Owner: RxndyOG
--

COPY public.deck (id, username, cardid1, cardid2, cardid3, cardid4) FROM stdin;
\.


--
-- Data for Name: trading; Type: TABLE DATA; Schema: public; Owner: RxndyOG
--

COPY public.trading (username, cardid, type, damage, id) FROM stdin;
\.


--
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: RxndyOG
--

COPY public.users (id, username, password, created_at, image, token, coins, wins, lose, bio) FROM stdin;
\.


--
-- Name: cards_id_seq; Type: SEQUENCE SET; Schema: public; Owner: RxndyOG
--

SELECT pg_catalog.setval('public.cards_id_seq', 3045, true);


--
-- Name: users_id_seq; Type: SEQUENCE SET; Schema: public; Owner: RxndyOG
--

SELECT pg_catalog.setval('public.users_id_seq', 250, true);


--
-- Name: cards cards_pkey; Type: CONSTRAINT; Schema: public; Owner: RxndyOG
--

ALTER TABLE ONLY public.cards
    ADD CONSTRAINT cards_pkey PRIMARY KEY (id);


--
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: RxndyOG
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);


--
-- Name: users users_unique; Type: CONSTRAINT; Schema: public; Owner: RxndyOG
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_unique UNIQUE (username);


--
-- PostgreSQL database dump complete
--

