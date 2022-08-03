using System.ComponentModel.DataAnnotations;

namespace CommandsService.Dtos
{
    public record CommandCreateDto
    (
        [Required] string HowTo,
        [Required] string CommandLine
    );
}