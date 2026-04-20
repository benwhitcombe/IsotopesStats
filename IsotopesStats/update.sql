ALTER TABLE seasonopponents ADD COLUMN IF NOT EXISTS name TEXT;

CREATE OR REPLACE VIEW v_game_summaries AS
SELECT g.id AS gameid,
    g.seasonid,
    g.gamenumber,
    g.date,
    g.diamond,
    g.opponentid,
    g.type AS gametype,
    g.isdeleted AS gameisdeleted,
    COALESCE(so.name, o.name, 'Unknown'::text) AS opponentname,
    (count(s.id))::integer AS playercount,
    (COALESCE(sum(s.r), (0)::bigint))::integer AS teamruns,
    (COALESCE(sum(((((s.h1b + s.h2b) + s.h3b) + s.h4b) + s.hr)), (0)::bigint))::integer AS teamhits,
    (COALESCE(sum(s.hr), (0)::bigint))::integer AS teamhrs,
    (COALESCE(sum(s.bb), (0)::bigint))::integer AS teambbs,
    (COALESCE(sum((((((((((s.h1b + s.h2b) + s.h3b) + s.h4b) + s.hr) + s.fc) + s.k) + s.kf) + s.go) + s.fo)), (0)::bigint))::integer AS teamab,
    (COALESCE(sum((((((((((((s.h1b + s.h2b) + s.h3b) + s.h4b) + s.hr) + s.fc) + s.k) + s.kf) + s.go) + s.fo) + s.bb) + s.sf)), (0)::bigint))::integer AS teampa,
    (COALESCE(sum(s.rbi), (0)::bigint))::integer AS teamrbi
   FROM games g
     LEFT JOIN opponents o ON g.opponentid = o.id
     LEFT JOIN seasonopponents so ON g.opponentid = so.opponentid AND g.seasonid = so.seasonid
     LEFT JOIN stats s ON g.id = s.gameid
  WHERE g.isdeleted = false
  GROUP BY g.id, g.seasonid, g.gamenumber, g.date, g.diamond, g.opponentid, g.type, g.isdeleted, so.name, o.name;

CREATE OR REPLACE VIEW v_game_stats_extended AS
SELECT s.id AS statid,
    s.playerid,
    s.gameid,
    s.bo,
    s.h1b,
    s.h2b,
    s.h3b,
    s.h4b,
    s.hr,
    s.fc,
    s.bb,
    s.sf,
    s.k,
    s.kf,
    s.go,
    s.fo,
    s.r,
    s.rbi,
    p.name AS playername,
    g.seasonid,
    g.gamenumber,
    g.date,
    g.diamond,
    g.opponentid,
    g.type AS gametype,
    g.isdeleted AS gameisdeleted,
    COALESCE(so.name, o.name, 'Unknown'::text) AS opponentname
   FROM stats s
     JOIN players p ON s.playerid = p.id
     JOIN games g ON s.gameid = g.id
     LEFT JOIN opponents o ON g.opponentid = o.id
     LEFT JOIN seasonopponents so ON g.opponentid = so.opponentid AND g.seasonid = so.seasonid;

CREATE OR REPLACE VIEW v_season_opponents_list AS
SELECT so.seasonid,
       so.opponentid,
       COALESCE(so.name, o.name, 'Unknown'::text) AS opponentname
FROM seasonopponents so
JOIN opponents o ON so.opponentid = o.id;