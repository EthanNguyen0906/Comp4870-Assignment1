using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Article
{
    [Key]
    public int ArticleId { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Body { get; set; } // Allows HTML content

    public DateTime CreateDate { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    [ForeignKey("User")]
    public string ContributorUsername { get; set; }

    public User Contributor { get; set; } // Navigation property
}
