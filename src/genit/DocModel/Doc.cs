﻿using System.Collections.Generic;

namespace Dyvenix.Genit.DocModel;

public class Doc
{
	public string Name { get; set; }
	public string Description { get; set; }
	public string Version { get; set; } = "1.0.0";

	public string OutputRootFolder { get; set; }
	public List<DbContextModel> DbContexts { get; set; }
}





