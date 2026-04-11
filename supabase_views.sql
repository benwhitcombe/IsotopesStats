-- STATISTICS VIEWS

-- View for Player Stats Summary (Aggregated by Season and Player)
CREATE OR REPLACE VIEW v_player_stats_summary AS
SELECT 
    g.seasonid,
    p.name as playername,
    COUNT(DISTINCT s.gameid) as gamesplayed,
    SUM(s.h1b)::INT as h1b,
    SUM(s.h2b)::INT as h2b,
    SUM(s.h3b)::INT as h3b,
    SUM(s.h4b)::INT as h4b,
    SUM(s.hr)::INT as hr,
    SUM(s.rbi)::INT as rbi,
    SUM(s.r)::INT as r,
    SUM(s.bb)::INT as bb,
    SUM(s.k)::INT as k,
    SUM(s.kf)::INT as kf,
    SUM(s.sf)::INT as sf,
    SUM(s.fc)::INT as fc,
    SUM(s.go)::INT as go,
    SUM(s.fo)::INT as fo
FROM stats s
JOIN players p ON s.playerid = p.id
JOIN games g ON s.gameid = g.id
WHERE p.name NOT LIKE 'spare' AND g.isdeleted = FALSE
GROUP BY g.seasonid, p.id, p.name;

-- View for Team Totals (Aggregated by Season)
CREATE OR REPLACE VIEW v_team_stats_summary AS
SELECT 
    g.seasonid,
    'TEAM TOTALS' as playername,
    SUM(s.h1b)::INT as h1b,
    SUM(s.h2b)::INT as h2b,
    SUM(s.h3b)::INT as h3b,
    SUM(s.h4b)::INT as h4b,
    SUM(s.hr)::INT as hr,
    SUM(s.rbi)::INT as rbi,
    SUM(s.r)::INT as r,
    SUM(s.bb)::INT as bb,
    SUM(s.k)::INT as k,
    SUM(s.kf)::INT as kf,
    SUM(s.sf)::INT as sf,
    SUM(s.fc)::INT as fc,
    SUM(s.go)::INT as go,
    SUM(s.fo)::INT as fo
FROM stats s
JOIN games g ON s.gameid = g.id
WHERE g.isdeleted = FALSE
GROUP BY g.seasonid;

-- View for Extended Game Stats (Joined with Player and Opponent info)
CREATE OR REPLACE VIEW v_game_stats_extended AS
SELECT 
    s.id as statid,
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
    p.name as playername,
    g.seasonid,
    g.gamenumber,
    g.date,
    g.diamond,
    g.opponentid,
    g.type as gametype,
    g.isdeleted as gameisdeleted,
    COALESCE(o.name, 'Unknown') as opponentname
FROM stats s
JOIN players p ON s.playerid = p.id
JOIN games g ON s.gameid = g.id
LEFT JOIN opponents o ON g.opponentid = o.id;

-- View for Player Game Logs
CREATE OR REPLACE VIEW v_player_game_logs AS
SELECT 
    s.id as statid,
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
    p.name as playername,
    g.seasonid,
    g.gamenumber,
    g.date,
    g.diamond,
    g.opponentid,
    g.type as gametype,
    COALESCE(o.name, 'Unknown') as opponentname
FROM stats s
JOIN games g ON s.gameid = g.id
JOIN players p ON s.playerid = p.id
LEFT JOIN opponents o ON g.opponentid = o.id
WHERE g.isdeleted = FALSE;


-- MANAGEMENT VIEWS

-- View for Games Management (Flattens Opponent Name for table display)
CREATE OR REPLACE VIEW v_games_management AS
SELECT 
    g.id,
    g.seasonid,
    g.gamenumber,
    g.date,
    g.diamond,
    g.opponentid,
    g.type as gametype,
    g.isdeleted,
    COALESCE(o.name, 'Unknown') as opponentname
FROM games g
LEFT JOIN opponents o ON g.opponentid = o.id
WHERE g.isdeleted = FALSE;

-- View for Season Rosters
CREATE OR REPLACE VIEW v_season_players_list AS
SELECT 
    sp.seasonid,
    p.id as playerid,
    p.name as playername
FROM seasonplayers sp
JOIN players p ON sp.playerid = p.id
WHERE p.isdeleted = FALSE;

-- View for User Role Management
CREATE OR REPLACE VIEW v_user_roles_summary AS
SELECT 
    u.id as userid,
    u.email,
    u.createdat,
    string_agg(r.name, ', ' ORDER BY r.name) as rolenames
FROM users u
LEFT JOIN useruserroles uur ON u.id = uur.userid
LEFT JOIN userroles r ON uur.roleid = r.id
WHERE u.isdeleted = FALSE
GROUP BY u.id, u.email, u.createdat;
