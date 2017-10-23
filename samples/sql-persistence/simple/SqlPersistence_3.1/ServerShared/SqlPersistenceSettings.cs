﻿using NServiceBus.Persistence.Sql;

#region SqlPersistenceSettings

[assembly: SqlPersistenceSettings(
    MsSqlServerScripts = true,
    MySqlScripts = true,
    OracleScripts = true,
    ScriptPromotionPath = "$(SolutionDir)PromotedSqlScripts")]

#endregion