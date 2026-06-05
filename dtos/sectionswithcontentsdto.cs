using System.Collections.Generic;
using UdemyBackend.DTOs.Content; // Assuming this namespace for the next step

namespace UdemyBackend.DTOs.Section
{
    public class SectionWithContentsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int SequenceOrder { get; set; }
        
        // Nested list of the flattened Content DTOs
        public List<ContentDto> Contents { get; set; } = new List<ContentDto>();
    }
}