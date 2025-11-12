// <copyright file="MSTestSettings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>


// Configure parallel test execution
// Workers = 0 means use the number of processors on the machine
[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.MethodLevel)]
