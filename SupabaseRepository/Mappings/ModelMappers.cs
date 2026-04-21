using IsotopesStats.Models;
using SupabaseRepository.Models;

namespace SupabaseRepository.Mappings;

public static class ModelMappers
{

    public static Game ToModel(this GameDto dto)
    {
        if (dto == null) return null!;
        return new Game
        {
            Id = dto.Id,
            SeasonId = dto.SeasonId,
            GameNumber = dto.GameNumber,
            Date = dto.Date,
            Diamond = dto.Diamond,
            IsHome = dto.IsHome,
            OpponentId = dto.OpponentId,
            Type = dto.Type,
            IsDeleted = dto.IsDeleted,
            VisitingTeamScore = dto.VisitingTeamScore,
            HomeTeamScore = dto.HomeTeamScore,
            Opponent = dto.Opponent?.ToModel()
        };
    }

    public static GameDto ToDto(this Game model)
    {
        if (model == null) return null!;
        return new GameDto
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

    public static GameManagementView ToModel(this GameManagementViewDto dto)
    {
        if (dto == null) return null!;
        return new GameManagementView
        {
            Id = dto.Id,
            SeasonId = dto.SeasonId,
            GameNumber = dto.GameNumber,
            Date = dto.Date,
            Diamond = dto.Diamond,
            IsHome = dto.IsHome,
            OpponentId = dto.OpponentId,
            GameType = dto.GameType,
            IsDeleted = dto.IsDeleted,
            OpponentName = dto.OpponentName
        };
    }

    public static GameManagementViewDto ToDto(this GameManagementView model)
    {
        if (model == null) return null!;
        return new GameManagementViewDto
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
            OpponentName = model.OpponentName
        };
    }

    public static GameSummaryView ToModel(this GameSummaryViewDto dto)
    {
        if (dto == null) return null!;
        return new GameSummaryView
        {
            GameId = dto.GameId,
            SeasonId = dto.SeasonId,
            GameNumber = dto.GameNumber,
            Date = dto.Date,
            Diamond = dto.Diamond,
            IsHome = dto.IsHome,
            OpponentId = dto.OpponentId,
            GameType = dto.GameType,
            GameIsDeleted = dto.GameIsDeleted,
            OpponentName = dto.OpponentName,
            PlayerCount = dto.PlayerCount,
            TeamRuns = dto.TeamRuns,
            TeamHits = dto.TeamHits,
            TeamHRs = dto.TeamHRs,
            TeamBBs = dto.TeamBBs,
            TeamAB = dto.TeamAB,
            TeamPA = dto.TeamPA,
            TeamRBI = dto.TeamRBI
        };
    }

    public static GameSummaryViewDto ToDto(this GameSummaryView model)
    {
        if (model == null) return null!;
        return new GameSummaryViewDto
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
            PlayerCount = model.PlayerCount,
            TeamRuns = model.TeamRuns,
            TeamHits = model.TeamHits,
            TeamHRs = model.TeamHRs,
            TeamBBs = model.TeamBBs,
            TeamAB = model.TeamAB,
            TeamPA = model.TeamPA,
            TeamRBI = model.TeamRBI
        };
    }

    public static Opponent ToModel(this OpponentDto dto)
    {
        if (dto == null) return null!;
        return new Opponent
        {
            Id = dto.Id,
            Name = dto.Name,
            IsDeleted = dto.IsDeleted
        };
    }

    public static OpponentDto ToDto(this Opponent model)
    {
        if (model == null) return null!;
        return new OpponentDto
        {
            Id = model.Id,
            Name = model.Name,
            IsDeleted = model.IsDeleted
        };
    }

    public static Permission ToModel(this PermissionDto dto)
    {
        if (dto == null) return null!;
        return new Permission
        {
            Id = (int)dto.Id,
            Name = dto.Name
        };
    }

    public static PermissionDto ToDto(this Permission model)
    {
        if (model == null) return null!;
        return new PermissionDto
        {
            Id = model.Id,
            Name = model.Name
        };
    }

    public static Player ToModel(this PlayerDto dto)
    {
        if (dto == null) return null!;
        return new Player
        {
            Id = dto.Id,
            Name = dto.Name,
            IsDeleted = dto.IsDeleted
        };
    }

    public static PlayerDto ToDto(this Player model)
    {
        if (model == null) return null!;
        return new PlayerDto
        {
            Id = model.Id,
            Name = model.Name,
            IsDeleted = model.IsDeleted
        };
    }

    public static PlayerStatsSummary ToModel(this PlayerStatsSummaryDto dto)
    {
        if (dto == null) return null!;
        return new PlayerStatsSummary
        {
            PlayerName = dto.PlayerName,
            GamesPlayed = dto.GamesPlayed,
            H1B = dto.H1B,
            H2B = dto.H2B,
            H3B = dto.H3B,
            H4B = dto.H4B,
            HR = dto.HR,
            FC = dto.FC,
            BB = dto.BB,
            SF = dto.SF,
            K = dto.K,
            KF = dto.KF,
            GO = dto.GO,
            FO = dto.FO,
            R = dto.R,
            RBI = dto.RBI
        };
    }

    public static PlayerStatsSummaryDto ToDto(this PlayerStatsSummary model)
    {
        if (model == null) return null!;
        return new PlayerStatsSummaryDto
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

    public static RolePermission ToModel(this RolePermissionDto dto)
    {
        if (dto == null) return null!;
        return new RolePermission
        {
            RoleId = (int)dto.RoleId,
            PermissionId = (int)dto.PermissionId,
            Permission = dto.Permission?.ToModel()
        };
    }

    public static RolePermissionDto ToDto(this RolePermission model)
    {
        if (model == null) return null!;
        return new RolePermissionDto
        {
            RoleId = model.RoleId,
            PermissionId = model.PermissionId
        };
    }

    public static Season ToModel(this SeasonDto dto)
    {
        if (dto == null) return null!;
        return new Season
        {
            Id = dto.Id,
            Name = dto.Name,
            IsDeleted = dto.IsDeleted
        };
    }

    public static SeasonDto ToDto(this Season model)
    {
        if (model == null) return null!;
        return new SeasonDto
        {
            Id = model.Id,
            Name = model.Name,
            IsDeleted = model.IsDeleted
        };
    }

    public static SeasonOpponents ToModel(this SeasonOpponentsDto dto)
    {
        if (dto == null) return null!;
        return new SeasonOpponents
        {
            SeasonId = dto.SeasonId,
            OpponentId = dto.OpponentId,
            Name = dto.Name
        };
    }

    public static SeasonOpponentsDto ToDto(this SeasonOpponents model)
    {
        if (model == null) return null!;
        return new SeasonOpponentsDto
        {
            SeasonId = model.SeasonId,
            OpponentId = model.OpponentId,
            Name = model.Name
        };
    }

    public static SeasonOpponentView ToModel(this SeasonOpponentViewDto dto)
    {
        if (dto == null) return null!;
        return new SeasonOpponentView
        {
            SeasonId = dto.SeasonId,
            OpponentId = dto.OpponentId,
            OpponentName = dto.OpponentName
        };
    }

    public static SeasonOpponentViewDto ToDto(this SeasonOpponentView model)
    {
        if (model == null) return null!;
        return new SeasonOpponentViewDto
        {
            SeasonId = model.SeasonId,
            OpponentId = model.OpponentId,
            OpponentName = model.OpponentName
        };
    }

    public static SeasonPlayers ToModel(this SeasonPlayersDto dto)
    {
        if (dto == null) return null!;
        return new SeasonPlayers
        {
            SeasonId = dto.SeasonId,
            PlayerId = dto.PlayerId
        };
    }

    public static SeasonPlayersDto ToDto(this SeasonPlayers model)
    {
        if (model == null) return null!;
        return new SeasonPlayersDto
        {
            SeasonId = model.SeasonId,
            PlayerId = model.PlayerId
        };
    }

    public static SeasonPlayerView ToModel(this SeasonPlayerViewDto dto)
    {
        if (dto == null) return null!;
        return new SeasonPlayerView
        {
            SeasonId = dto.SeasonId,
            PlayerId = dto.PlayerId,
            PlayerName = dto.PlayerName
        };
    }

    public static SeasonPlayerViewDto ToDto(this SeasonPlayerView model)
    {
        if (model == null) return null!;
        return new SeasonPlayerViewDto
        {
            SeasonId = model.SeasonId,
            PlayerId = model.PlayerId,
            PlayerName = model.PlayerName
        };
    }

    public static StatEntry ToModel(this StatEntryDto dto)
    {
        if (dto == null) return null!;
        return new StatEntry
        {
            Id = dto.Id,
            PlayerId = dto.PlayerId,
            GameId = dto.GameId,
            BO = dto.BO,
            H1B = dto.H1B,
            H2B = dto.H2B,
            H3B = dto.H3B,
            H4B = dto.H4B,
            HR = dto.HR,
            FC = dto.FC,
            BB = dto.BB,
            SF = dto.SF,
            K = dto.K,
            KF = dto.KF,
            GO = dto.GO,
            FO = dto.FO,
            R = dto.R,
            RBI = dto.RBI,
            Player = dto.Player?.ToModel(),
            Game = dto.Game?.ToModel()
        };
    }

    public static StatEntryDto ToDto(this StatEntry model)
    {
        if (model == null) return null!;
        return new StatEntryDto
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
            Player = model.Player?.ToDto(),
            Game = model.Game?.ToDto()
        };
    }

    public static User ToModel(this UserDto dto)
    {
        if (dto == null) return null!;
        return new User
        {
            Id = dto.Id.ToString(),
            Email = dto.Email,
            PasswordHash = dto.PasswordHash,
            CreatedAt = dto.CreatedAt,
            IsDeleted = dto.IsDeleted
        };
    }

    public static UserDto ToDto(this User model)
    {
        if (model == null) return null!;
        return new UserDto
        {
            Id = string.IsNullOrEmpty(model.Id) ? Guid.Empty : Guid.Parse(model.Id),
            Email = model.Email,
            PasswordHash = model.PasswordHash,
            CreatedAt = model.CreatedAt,
            IsDeleted = model.IsDeleted
        };
    }

    public static UserLog ToModel(this UserLogDto dto)
    {
        if (dto == null) return null!;
        return new UserLog
        {
            Id = dto.Id,
            UserId = dto.UserId,
            UserEmail = dto.UserEmail,
            Action = dto.Action,
            EntityType = dto.EntityType,
            EntityId = dto.EntityId,
            Description = dto.Description,
            Timestamp = dto.Timestamp
        };
    }

    public static UserLogDto ToDto(this UserLog model)
    {
        if (model == null) return null!;
        return new UserLogDto
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

    public static UserRole ToModel(this UserRoleDto dto)
    {
        if (dto == null) return null!;
        UserRole model = new UserRole
        {
            Id = (int)dto.Id,
            Name = dto.Name,
            IsDeleted = dto.IsDeleted
        };

        if (dto.RolePermissions != null && dto.RolePermissions.Any())
        {
            model.Permissions = dto.RolePermissions
                .Where(rp => rp.Permission != null)
                .Select(rp => rp.Permission!.ToModel())
                .ToList();
        }

        return model;
    }

    public static UserRoleDto ToDto(this UserRole model)
    {
        if (model == null) return null!;
        return new UserRoleDto
        {
            Id = model.Id,
            Name = model.Name,
            IsDeleted = model.IsDeleted
        };
    }

    public static UserRolesSummaryView ToModel(this UserRolesSummaryViewDto dto)
    {
        if (dto == null) return null!;
        return new UserRolesSummaryView
        {
            Id = dto.Id,
            Email = dto.Email,
            CreatedAt = dto.CreatedAt,
            RoleNames = dto.RoleNames,
            IsDeleted = dto.IsDeleted
        };
    }

    public static UserRolesSummaryViewDto ToDto(this UserRolesSummaryView model)
    {
        if (model == null) return null!;
        return new UserRolesSummaryViewDto
        {
            Id = model.Id,
            Email = model.Email,
            CreatedAt = model.CreatedAt,
            RoleNames = model.RoleNames,
            IsDeleted = model.IsDeleted
        };
    }

    public static UserUserRoles ToModel(this UserUserRolesDto dto)
    {
        if (dto == null) return null!;
        return new UserUserRoles
        {
            UserId = dto.UserId.ToString(),
            RoleId = (int)dto.RoleId
        };
    }

    public static UserUserRolesDto ToDto(this UserUserRoles model)
    {
        if (model == null) return null!;
        return new UserUserRolesDto
        {
            UserId = Guid.Parse(model.UserId),
            RoleId = model.RoleId
        };
    }

    public static TeamStatsSummary ToModel(this TeamStatsSummaryDto dto)
    {
        if (dto == null) return null!;
        return new TeamStatsSummary
        {
            PlayerName = dto.PlayerName,
            GamesPlayed = dto.GamesPlayed,
            H1B = dto.H1B,
            H2B = dto.H2B,
            H3B = dto.H3B,
            H4B = dto.H4B,
            HR = dto.HR,
            FC = dto.FC,
            BB = dto.BB,
            SF = dto.SF,
            K = dto.K,
            KF = dto.KF,
            GO = dto.GO,
            FO = dto.FO,
            R = dto.R,
            RBI = dto.RBI
        };
    }

    public static GameStatsExtendedView ToModel(this GameStatsExtendedViewDto dto)
    {
        if (dto == null) return null!;
        return new GameStatsExtendedView
        {
            Id = dto.Id,
            PlayerId = dto.PlayerId,
            GameId = dto.GameId,
            BO = dto.BO,
            H1B = dto.H1B,
            H2B = dto.H2B,
            H3B = dto.H3B,
            H4B = dto.H4B,
            HR = dto.HR,
            FC = dto.FC,
            BB = dto.BB,
            SF = dto.SF,
            K = dto.K,
            KF = dto.KF,
            GO = dto.GO,
            FO = dto.FO,
            R = dto.R,
            RBI = dto.RBI,
            Player = dto.Player?.ToModel(),
            Game = dto.Game?.ToModel(),
            PlayerName = dto.PlayerName,
            SeasonId = dto.SeasonId,
            GameNumber = dto.GameNumber,
            Date = dto.Date,
            Diamond = dto.Diamond,
            IsHome = dto.IsHome,
            OpponentId = dto.OpponentId,
            GameType = dto.GameType,
            GameIsDeleted = dto.GameIsDeleted,
            OpponentName = dto.OpponentName
        };
    }

}
