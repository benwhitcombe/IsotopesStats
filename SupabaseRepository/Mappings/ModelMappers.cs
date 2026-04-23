using IsotopesStats.Models;
using SupabaseRepository.Models;
using IsotopesStats.Domain.Services;

namespace SupabaseRepository.Mappings;

public static class ModelMappers
{
    public static Game ToModel(this GameDTO DTO)
    {
        if (DTO == null) return null!;
        return new Game
        {
            Id = DTO.Id,
            SeasonId = DTO.SeasonId,
            GameNumber = DTO.GameNumber,
            Date = DateTimeService.ToWhitbyTime(DTO.Date),
            Diamond = DTO.Diamond,
            IsHome = DTO.IsHome,
            OpponentId = DTO.OpponentId,
            Type = DTO.Type,
            IsDeleted = DTO.IsDeleted,
            VisitingTeamScore = DTO.VisitingTeamScore,
            HomeTeamScore = DTO.HomeTeamScore,
            Opponent = DTO.Opponent?.ToModel()
        };
    }

    public static GameDTO ToDTO(this Game model)
    {
        if (model == null) return null!;
        return new GameDTO
        {
            Id = model.Id,
            SeasonId = model.SeasonId,
            GameNumber = model.GameNumber,
            Date = model.Date,
            Diamond = model.Diamond,
            IsHome = model.IsHome,
            OpponentId = model.OpponentId,
            Type = model.Type,
            IsDeleted = model.IsDeleted,
            VisitingTeamScore = model.VisitingTeamScore,
            HomeTeamScore = model.HomeTeamScore
        };
    }

    public static GameManagementView ToModel(this GameManagementViewDTO DTO)
    {
        if (DTO == null) return null!;
        return new GameManagementView
        {
            Id = DTO.Id,
            SeasonId = DTO.SeasonId,
            GameNumber = DTO.GameNumber,
            Date = DateTimeService.ToWhitbyTime(DTO.Date),
            Diamond = DTO.Diamond,
            IsHome = DTO.IsHome,
            OpponentId = DTO.OpponentId,
            GameType = DTO.GameType,
            IsDeleted = DTO.IsDeleted,
            OpponentName = DTO.OpponentName,
            OpponentShortName = DTO.OpponentShortName
        };
    }

    public static GameManagementViewDTO ToDTO(this GameManagementView model)
    {
        if (model == null) return null!;
        return new GameManagementViewDTO
        {
            Id = model.Id,
            SeasonId = model.SeasonId,
            GameNumber = model.GameNumber,
            Date = model.Date,
            Diamond = model.Diamond,
            IsHome = model.IsHome,
            OpponentId = model.OpponentId,
            GameType = model.GameType,
            IsDeleted = model.IsDeleted,
            OpponentName = model.OpponentName,
            OpponentShortName = model.OpponentShortName
        };
    }

    public static GameSummaryView ToModel(this GameSummaryViewDTO DTO)
    {
        if (DTO == null) return null!;
        return new GameSummaryView
        {
            GameId = DTO.GameId,
            SeasonId = DTO.SeasonId,
            GameNumber = DTO.GameNumber,
            Date = DateTimeService.ToWhitbyTime(DTO.Date),
            Diamond = DTO.Diamond,
            IsHome = DTO.IsHome,
            OpponentId = DTO.OpponentId,
            GameType = DTO.GameType,
            GameIsDeleted = DTO.GameIsDeleted,
            OpponentName = DTO.OpponentName,
            OpponentShortName = DTO.OpponentShortName,
            PlayerCount = DTO.PlayerCount,
            TeamRuns = DTO.TeamRuns,
            TeamHits = DTO.TeamHits,
            TeamHRs = DTO.TeamHRs,
            TeamBBs = DTO.TeamBBs,
            TeamAB = DTO.TeamAB,
            TeamPA = DTO.TeamPA,
            TeamRBI = DTO.TeamRBI,
            VisitingTeamScore = DTO.VisitingTeamScore,
            HomeTeamScore = DTO.HomeTeamScore
        };
    }

    public static GameSummaryViewDTO ToDTO(this GameSummaryView model)
    {
        if (model == null) return null!;
        return new GameSummaryViewDTO
        {
            GameId = model.GameId,
            SeasonId = model.SeasonId,
            GameNumber = model.GameNumber,
            Date = model.Date,
            Diamond = model.Diamond,
            IsHome = model.IsHome,
            OpponentId = model.OpponentId,
            GameType = model.GameType,
            GameIsDeleted = model.GameIsDeleted,
            OpponentName = model.OpponentName,
            OpponentShortName = model.OpponentShortName,
            PlayerCount = model.PlayerCount,
            TeamRuns = model.TeamRuns,
            TeamHits = model.TeamHits,
            TeamHRs = model.TeamHRs,
            TeamBBs = model.TeamBBs,
            TeamAB = model.TeamAB,
            TeamPA = model.TeamPA,
            TeamRBI = model.TeamRBI,
            VisitingTeamScore = model.VisitingTeamScore,
            HomeTeamScore = model.HomeTeamScore
        };
    }

    public static Opponent ToModel(this OpponentDTO DTO)
    {
        if (DTO == null) return null!;
        return new Opponent
        {
            Id = DTO.Id,
            Name = DTO.Name,
            ShortName = DTO.ShortName,
            IsDeleted = DTO.IsDeleted
        };
    }

    public static OpponentDTO ToDTO(this Opponent model)
    {
        if (model == null) return null!;
        return new OpponentDTO
        {
            Id = model.Id,
            Name = model.Name,
            ShortName = model.ShortName,
            IsDeleted = model.IsDeleted
        };
    }

    public static Permission ToModel(this PermissionDTO DTO)
    {
        if (DTO == null) return null!;
        return new Permission
        {
            Id = (int)DTO.Id,
            Name = DTO.Name
        };
    }

    public static PermissionDTO ToDTO(this Permission model)
    {
        if (model == null) return null!;
        return new PermissionDTO
        {
            Id = model.Id,
            Name = model.Name
        };
    }

    public static Player ToModel(this PlayerDTO DTO)
    {
        if (DTO == null) return null!;
        return new Player
        {
            Id = DTO.Id,
            Name = DTO.Name,
            IsDeleted = DTO.IsDeleted
        };
    }

    public static PlayerDTO ToDTO(this Player model)
    {
        if (model == null) return null!;
        return new PlayerDTO
        {
            Id = model.Id,
            Name = model.Name,
            IsDeleted = model.IsDeleted
        };
    }

    public static PlayerStatsSummary ToModel(this PlayerStatsSummaryDTO DTO)
    {
        if (DTO == null) return null!;
        return new PlayerStatsSummary
        {
            PlayerName = DTO.PlayerName,
            GamesPlayed = DTO.GamesPlayed,
            H1B = DTO.H1B,
            H2B = DTO.H2B,
            H3B = DTO.H3B,
            H4B = DTO.H4B,
            HR = DTO.HR,
            FC = DTO.FC,
            BB = DTO.BB,
            SF = DTO.SF,
            K = DTO.K,
            KF = DTO.KF,
            GO = DTO.GO,
            FO = DTO.FO,
            R = DTO.R,
            RBI = DTO.RBI
        };
    }

    public static PlayerStatsSummaryDTO ToDTO(this PlayerStatsSummary model)
    {
        if (model == null) return null!;
        return new PlayerStatsSummaryDTO
        {
            PlayerName = model.PlayerName,
            GamesPlayed = model.GamesPlayed,
            H1B = model.H1B,
            H2B = model.H2B,
            H3B = model.H3B,
            H4B = model.H4B,
            HR = model.HR,
            FC = model.FC,
            BB = model.BB,
            SF = model.SF,
            K = model.K,
            KF = model.KF,
            GO = model.GO,
            FO = model.FO,
            R = model.R,
            RBI = model.RBI
        };
    }

    public static RolePermission ToModel(this RolePermissionDTO DTO)
    {
        if (DTO == null) return null!;
        return new RolePermission
        {
            RoleId = (int)DTO.RoleId,
            PermissionId = (int)DTO.PermissionId,
            Permission = DTO.Permission?.ToModel()
        };
    }

    public static RolePermissionDTO ToDTO(this RolePermission model)
    {
        if (model == null) return null!;
        return new RolePermissionDTO
        {
            RoleId = model.RoleId,
            PermissionId = model.PermissionId
        };
    }

    public static Season ToModel(this SeasonDTO DTO)
    {
        if (DTO == null) return null!;
        return new Season
        {
            Id = DTO.Id,
            Name = DTO.Name,
            IsDeleted = DTO.IsDeleted
        };
    }

    public static SeasonDTO ToDTO(this Season model)
    {
        if (model == null) return null!;
        return new SeasonDTO
        {
            Id = model.Id,
            Name = model.Name,
            IsDeleted = model.IsDeleted
        };
    }

    public static SeasonOpponents ToModel(this SeasonOpponentsDTO DTO)
    {
        if (DTO == null) return null!;
        return new SeasonOpponents
        {
            SeasonId = DTO.SeasonId,
            OpponentId = DTO.OpponentId,
            Name = DTO.Name,
            ShortName = DTO.ShortName
        };
    }

    public static SeasonOpponentsDTO ToDTO(this SeasonOpponents model)
    {
        if (model == null) return null!;
        return new SeasonOpponentsDTO
        {
            SeasonId = model.SeasonId,
            OpponentId = model.OpponentId,
            Name = model.Name,
            ShortName = model.ShortName
        };
    }

    public static SeasonOpponentView ToModel(this SeasonOpponentViewDTO DTO)
    {
        if (DTO == null) return null!;
        return new SeasonOpponentView
        {
            SeasonId = DTO.SeasonId,
            OpponentId = DTO.OpponentId,
            OpponentName = DTO.OpponentName,
            OpponentShortName = DTO.OpponentShortName
        };
    }

    public static SeasonOpponentViewDTO ToDTO(this SeasonOpponentView model)
    {
        if (model == null) return null!;
        return new SeasonOpponentViewDTO
        {
            SeasonId = model.SeasonId,
            OpponentId = model.OpponentId,
            OpponentName = model.OpponentName
        };
    }

    public static SeasonPlayers ToModel(this SeasonPlayersDTO DTO)
    {
        if (DTO == null) return null!;
        return new SeasonPlayers
        {
            SeasonId = DTO.SeasonId,
            PlayerId = DTO.PlayerId
        };
    }

    public static SeasonPlayersDTO ToDTO(this SeasonPlayers model)
    {
        if (model == null) return null!;
        return new SeasonPlayersDTO
        {
            SeasonId = model.SeasonId,
            PlayerId = model.PlayerId
        };
    }

    public static SeasonPlayerView ToModel(this SeasonPlayerViewDTO DTO)
    {
        if (DTO == null) return null!;
        return new SeasonPlayerView
        {
            SeasonId = DTO.SeasonId,
            PlayerId = DTO.PlayerId,
            PlayerName = DTO.PlayerName
        };
    }

    public static SeasonPlayerViewDTO ToDTO(this SeasonPlayerView model)
    {
        if (model == null) return null!;
        return new SeasonPlayerViewDTO
        {
            SeasonId = model.SeasonId,
            PlayerId = model.PlayerId,
            PlayerName = model.PlayerName
        };
    }

    public static StatEntry ToModel(this StatEntryDTO DTO)
    {
        if (DTO == null) return null!;
        return new StatEntry
        {
            Id = DTO.Id,
            PlayerId = DTO.PlayerId,
            GameId = DTO.GameId,
            BO = DTO.BO,
            H1B = DTO.H1B,
            H2B = DTO.H2B,
            H3B = DTO.H3B,
            H4B = DTO.H4B,
            HR = DTO.HR,
            FC = DTO.FC,
            BB = DTO.BB,
            SF = DTO.SF,
            K = DTO.K,
            KF = DTO.KF,
            GO = DTO.GO,
            FO = DTO.FO,
            R = DTO.R,
            RBI = DTO.RBI,
            Player = DTO.Player?.ToModel(),
            Game = DTO.Game?.ToModel()
        };
    }

    public static StatEntryDTO ToDTO(this StatEntry model)
    {
        if (model == null) return null!;
        return new StatEntryDTO
        {
            Id = model.Id,
            PlayerId = model.PlayerId,
            GameId = model.GameId,
            BO = model.BO,
            H1B = model.H1B,
            H2B = model.H2B,
            H3B = model.H3B,
            H4B = model.H4B,
            HR = model.HR,
            FC = model.FC,
            BB = model.BB,
            SF = model.SF,
            K = model.K,
            KF = model.KF,
            GO = model.GO,
            FO = model.FO,
            R = model.R,
            RBI = model.RBI,
            Player = model.Player?.ToDTO(),
            Game = model.Game?.ToDTO()
        };
    }

    public static User ToModel(this UserDTO DTO)
    {
        if (DTO == null) return null!;
        return new User
        {
            Id = DTO.Id,
            Email = DTO.Email,
            PasswordHash = DTO.PasswordHash,
            CreatedAt = DateTimeService.ToWhitbyTime(DTO.CreatedAt),
            IsDeleted = DTO.IsDeleted
        };
    }

    public static UserDTO ToDTO(this User model)
    {
        if (model == null) return null!;
        return new UserDTO
        {
            Id = model.Id,
            Email = model.Email,
            PasswordHash = model.PasswordHash,
            CreatedAt = model.CreatedAt,
            IsDeleted = model.IsDeleted
        };
    }

    public static UserLog ToModel(this UserLogDTO DTO)
    {
        if (DTO == null) return null!;
        return new UserLog
        {
            Id = DTO.Id,
            UserId = DTO.UserId,
            UserEmail = DTO.UserEmail,
            Action = DTO.Action,
            EntityType = DTO.EntityType,
            EntityId = DTO.EntityId,
            Description = DTO.Description,
            Timestamp = DateTimeService.ToWhitbyTime(DTO.Timestamp)
        };
    }

    public static UserLogDTO ToDTO(this UserLog model)
    {
        if (model == null) return null!;
        return new UserLogDTO
        {
            Id = model.Id,
            UserId = model.UserId,
            UserEmail = model.UserEmail,
            Action = model.Action,
            EntityType = model.EntityType,
            EntityId = model.EntityId,
            Description = model.Description,
            Timestamp = model.Timestamp
        };
    }

    public static UserRole ToModel(this UserRoleDTO DTO)
    {
        if (DTO == null) return null!;
        UserRole model = new UserRole
        {
            Id = (int)DTO.Id,
            Name = DTO.Name,
            IsDeleted = DTO.IsDeleted
        };

        if (DTO.RolePermissions != null && DTO.RolePermissions.Any())
        {
            model.Permissions = DTO.RolePermissions
                .Where(rp => rp.Permission != null)
                .Select(rp => rp.Permission!.ToModel())
                .ToList();
        }

        return model;
    }

    public static UserRoleDTO ToDTO(this UserRole model)
    {
        if (model == null) return null!;
        return new UserRoleDTO
        {
            Id = model.Id,
            Name = model.Name,
            IsDeleted = model.IsDeleted
        };
    }

    public static UserRolesSummaryView ToModel(this UserRolesSummaryViewDTO DTO)
    {
        if (DTO == null) return null!;
        return new UserRolesSummaryView
        {
            Id = DTO.Id,
            Email = DTO.Email,
            CreatedAt = DateTimeService.ToWhitbyTime(DTO.CreatedAt),
            RoleNames = DTO.RoleNames,
            IsDeleted = DTO.IsDeleted
        };
    }

    public static UserRolesSummaryViewDTO ToDTO(this UserRolesSummaryView model)
    {
        if (model == null) return null!;
        return new UserRolesSummaryViewDTO
        {
            Id = model.Id,
            Email = model.Email,
            CreatedAt = model.CreatedAt,
            RoleNames = model.RoleNames,
            IsDeleted = model.IsDeleted
        };
    }

    public static UserUserRoles ToModel(this UserUserRolesDTO DTO)
    {
        if (DTO == null) return null!;
        return new UserUserRoles
        {
            UserId = DTO.UserId.ToString(),
            RoleId = (int)DTO.RoleId
        };
    }

    public static UserUserRolesDTO ToDTO(this UserUserRoles model)
    {
        if (model == null) return null!;
        return new UserUserRolesDTO
        {
            UserId = Guid.Parse(model.UserId),
            RoleId = model.RoleId
        };
    }

    public static TeamStatsSummary ToModel(this TeamStatsSummaryDTO DTO)
    {
        if (DTO == null) return null!;
        return new TeamStatsSummary
        {
            PlayerName = DTO.PlayerName,
            GamesPlayed = DTO.GamesPlayed,
            H1B = DTO.H1B,
            H2B = DTO.H2B,
            H3B = DTO.H3B,
            H4B = DTO.H4B,
            HR = DTO.HR,
            FC = DTO.FC,
            BB = DTO.BB,
            SF = DTO.SF,
            K = DTO.K,
            KF = DTO.KF,
            GO = DTO.GO,
            FO = DTO.FO,
            R = DTO.R,
            RBI = DTO.RBI
        };
    }

    public static GameStatsExtendedView ToModel(this GameStatsExtendedViewDTO DTO)
    {
        if (DTO == null) return null!;
        return new GameStatsExtendedView
        {
            Id = DTO.Id,
            PlayerId = DTO.PlayerId,
            GameId = DTO.GameId,
            BO = DTO.BO,
            H1B = DTO.H1B,
            H2B = DTO.H2B,
            H3B = DTO.H3B,
            H4B = DTO.H4B,
            HR = DTO.HR,
            FC = DTO.FC,
            BB = DTO.BB,
            SF = DTO.SF,
            K = DTO.K,
            KF = DTO.KF,
            GO = DTO.GO,
            FO = DTO.FO,
            R = DTO.R,
            RBI = DTO.RBI,
            Player = DTO.Player?.ToModel(),
            Game = DTO.Game?.ToModel(),
            PlayerName = DTO.PlayerName,
            SeasonId = DTO.SeasonId,
            GameNumber = DTO.GameNumber,
            Date = DateTimeService.ToWhitbyTime(DTO.Date),
            Diamond = DTO.Diamond,
            IsHome = DTO.IsHome,
            OpponentId = DTO.OpponentId,
            GameType = DTO.GameType,
            GameIsDeleted = DTO.GameIsDeleted,
            OpponentName = DTO.OpponentName,
            OpponentShortName = DTO.OpponentShortName
        };
    }

}
