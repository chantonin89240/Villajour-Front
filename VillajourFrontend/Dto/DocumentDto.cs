﻿using VillajourFrontend.Entity;

namespace VillajourFrontend.Dto;

public class DocumentDto
{
public int Id { get; set; }
public DateOnly Date { get; set; }
public string? Title { get; set; }
public string? Description { get; set; }
public string? DocumentUrl { get; set; }
public DocumentType? DocumentType { get; set; }
public Mairie? Mairie { get; set; }
}
