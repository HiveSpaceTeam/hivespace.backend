﻿using HiveSpace.Common.Exceptions.Models;

namespace HiveSpace.Common.Exceptions;

public class ForbiddenException : BaseException
{
    private static readonly int _httpCode = 403;

    public ForbiddenException(List<ErrorCode> errorCodeList, bool? enableData = false)
        : base(errorCodeList, _httpCode, enableData)
    {
    }

    public ForbiddenException(List<ErrorCode> errorCodeList, Exception inner, bool? enableData = false)
        : base(errorCodeList, inner, _httpCode, enableData)
    {
    }
}
