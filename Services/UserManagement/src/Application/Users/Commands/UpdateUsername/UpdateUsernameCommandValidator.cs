﻿using FluentValidation;

namespace Application.Users.Commands.UpdateUsername;

/// <summary>
///     UpdateUsernameCommand validator.
/// </summary>
public class UpdateUsernameCommandValidator : AbstractValidator<UpdateUsernameCommand>
{
    /// <summary>
    ///     Initializes UpdateUsernameCommandValidator.
    /// </summary>
    public UpdateUsernameCommandValidator()
    {
        RuleFor(x => x.Username)
            .MaximumLength(256);
    }
}