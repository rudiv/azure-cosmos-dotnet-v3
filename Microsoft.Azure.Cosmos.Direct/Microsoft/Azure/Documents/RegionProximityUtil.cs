using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Documents;

internal static class RegionProximityUtil
{
	internal static Dictionary<string, Dictionary<string, long>> SourceRegionToTargetRegionsRTTInMs = new Dictionary<string, Dictionary<string, long>>
	{
		{
			"Australia Central",
			new Dictionary<string, long>
			{
				{ "Australia Central", 0L },
				{ "Australia Central 2", 2L },
				{ "Australia East", 6L },
				{ "Australia Southeast", 18L },
				{ "Austria East", 240L },
				{ "Brazil South", 314L },
				{ "Brazil Southeast", 314L },
				{ "Canada Central", 204L },
				{ "Canada East", 214L },
				{ "Central India", 144L },
				{ "Central US", 184L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 120L },
				{ "East US", 205L },
				{ "East US 2", 200L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 242L },
				{ "France South", 230L },
				{ "Germany North", 254L },
				{ "Germany West Central", 246L },
				{ "Indonesia Central", 94L },
				{ "Israel Central", 168L },
				{ "Italy North", 240L },
				{ "Japan East", 125L },
				{ "Japan West", 130L },
				{ "Jio India Central", 144L },
				{ "Jio India West", 144L },
				{ "Korea Central", 154L },
				{ "Korea South", 148L },
				{ "Malaysia South", 94L },
				{ "Mexico Central", 162L },
				{ "New Zealand North", 100L },
				{ "North Central US", 192L },
				{ "North Europe", 262L },
				{ "Norway East", 272L },
				{ "Norway West", 266L },
				{ "Poland Central", 252L },
				{ "Qatar Central", 168L },
				{ "South Africa North", 390L },
				{ "South Africa West", 394L },
				{ "South Central US", 174L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 94L },
				{ "South India", 126L },
				{ "Spain Central", 242L },
				{ "Sweden Central", 272L },
				{ "Sweden South", 272L },
				{ "Switzerland North", 240L },
				{ "Switzerland West", 236L },
				{ "Taiwan North", 120L },
				{ "Taiwan Northwest", 120L },
				{ "UAE Central", 168L },
				{ "UAE North", 168L },
				{ "UK South", 250L },
				{ "UK West", 252L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 170L },
				{ "West Europe", 252L },
				{ "West India", 142L },
				{ "West US", 142L },
				{ "West US 2", 162L },
				{ "West US 3", 162L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Australia Central 2",
			new Dictionary<string, long>
			{
				{ "Australia Central", 2L },
				{ "Australia Central 2", 0L },
				{ "Australia East", 6L },
				{ "Australia Southeast", 18L },
				{ "Austria East", 240L },
				{ "Brazil South", 313L },
				{ "Brazil Southeast", 313L },
				{ "Canada Central", 204L },
				{ "Canada East", 214L },
				{ "Central India", 144L },
				{ "Central US", 184L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 120L },
				{ "East US", 205L },
				{ "East US 2", 200L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 242L },
				{ "France South", 230L },
				{ "Germany North", 256L },
				{ "Germany West Central", 246L },
				{ "Indonesia Central", 92L },
				{ "Israel Central", 168L },
				{ "Italy North", 240L },
				{ "Japan East", 124L },
				{ "Japan West", 132L },
				{ "Jio India Central", 144L },
				{ "Jio India West", 144L },
				{ "Korea Central", 154L },
				{ "Korea South", 148L },
				{ "Malaysia South", 92L },
				{ "Mexico Central", 162L },
				{ "New Zealand North", 2L },
				{ "North Central US", 192L },
				{ "North Europe", 262L },
				{ "Norway East", 272L },
				{ "Norway West", 266L },
				{ "Poland Central", 252L },
				{ "Qatar Central", 168L },
				{ "South Africa North", 391L },
				{ "South Africa West", 392L },
				{ "South Central US", 174L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 92L },
				{ "South India", 124L },
				{ "Spain Central", 242L },
				{ "Sweden Central", 272L },
				{ "Sweden South", 272L },
				{ "Switzerland North", 240L },
				{ "Switzerland West", 236L },
				{ "Taiwan North", 120L },
				{ "Taiwan Northwest", 120L },
				{ "UAE Central", 168L },
				{ "UAE North", 168L },
				{ "UK South", 248L },
				{ "UK West", 252L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 170L },
				{ "West Europe", 252L },
				{ "West India", 142L },
				{ "West US", 142L },
				{ "West US 2", 162L },
				{ "West US 3", 162L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Australia East",
			new Dictionary<string, long>
			{
				{ "Australia Central", 6L },
				{ "Australia Central 2", 6L },
				{ "Australia East", 0L },
				{ "Australia Southeast", 14L },
				{ "Austria East", 236L },
				{ "Brazil South", 308L },
				{ "Brazil Southeast", 308L },
				{ "Canada Central", 200L },
				{ "Canada East", 210L },
				{ "Central India", 140L },
				{ "Central US", 180L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 116L },
				{ "East US", 200L },
				{ "East US 2", 196L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 238L },
				{ "France South", 224L },
				{ "Germany North", 250L },
				{ "Germany West Central", 240L },
				{ "Indonesia Central", 88L },
				{ "Israel Central", 164L },
				{ "Italy North", 236L },
				{ "Japan East", 118L },
				{ "Japan West", 126L },
				{ "Jio India Central", 140L },
				{ "Jio India West", 140L },
				{ "Korea Central", 150L },
				{ "Korea South", 143L },
				{ "Malaysia South", 88L },
				{ "Mexico Central", 158L },
				{ "New Zealand North", 6L },
				{ "North Central US", 187L },
				{ "North Europe", 258L },
				{ "Norway East", 266L },
				{ "Norway West", 260L },
				{ "Poland Central", 248L },
				{ "Qatar Central", 164L },
				{ "South Africa North", 386L },
				{ "South Africa West", 390L },
				{ "South Central US", 170L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 88L },
				{ "South India", 120L },
				{ "Spain Central", 238L },
				{ "Sweden Central", 266L },
				{ "Sweden South", 266L },
				{ "Switzerland North", 236L },
				{ "Switzerland West", 232L },
				{ "Taiwan North", 116L },
				{ "Taiwan Northwest", 116L },
				{ "UAE Central", 164L },
				{ "UAE North", 163L },
				{ "UK South", 246L },
				{ "UK West", 248L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 166L },
				{ "West Europe", 248L },
				{ "West India", 138L },
				{ "West US", 138L },
				{ "West US 2", 158L },
				{ "West US 3", 158L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Australia Southeast",
			new Dictionary<string, long>
			{
				{ "Australia Central", 18L },
				{ "Australia Central 2", 18L },
				{ "Australia East", 14L },
				{ "Australia Southeast", 0L },
				{ "Austria East", 232L },
				{ "Brazil South", 320L },
				{ "Brazil Southeast", 320L },
				{ "Canada Central", 210L },
				{ "Canada East", 220L },
				{ "Central India", 136L },
				{ "Central US", 190L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 118L },
				{ "East US", 212L },
				{ "East US 2", 206L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 235L },
				{ "France South", 222L },
				{ "Germany North", 246L },
				{ "Germany West Central", 238L },
				{ "Indonesia Central", 85L },
				{ "Israel Central", 160L },
				{ "Italy North", 232L },
				{ "Japan East", 130L },
				{ "Japan West", 138L },
				{ "Jio India Central", 136L },
				{ "Jio India West", 136L },
				{ "Korea Central", 146L },
				{ "Korea South", 140L },
				{ "Malaysia South", 85L },
				{ "Mexico Central", 168L },
				{ "New Zealand North", 18L },
				{ "North Central US", 198L },
				{ "North Europe", 260L },
				{ "Norway East", 264L },
				{ "Norway West", 258L },
				{ "Poland Central", 244L },
				{ "Qatar Central", 160L },
				{ "South Africa North", 382L },
				{ "South Africa West", 386L },
				{ "South Central US", 180L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 85L },
				{ "South India", 118L },
				{ "Spain Central", 235L },
				{ "Sweden Central", 264L },
				{ "Sweden South", 264L },
				{ "Switzerland North", 232L },
				{ "Switzerland West", 228L },
				{ "Taiwan North", 118L },
				{ "Taiwan Northwest", 118L },
				{ "UAE Central", 160L },
				{ "UAE North", 160L },
				{ "UK South", 242L },
				{ "UK West", 245L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 176L },
				{ "West Europe", 244L },
				{ "West India", 134L },
				{ "West US", 148L },
				{ "West US 2", 168L },
				{ "West US 3", 168L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Austria East",
			new Dictionary<string, long>
			{
				{ "Australia Central", 240L },
				{ "Australia Central 2", 240L },
				{ "Australia East", 236L },
				{ "Australia Southeast", 232L },
				{ "Austria East", 0L },
				{ "Brazil South", 198L },
				{ "Brazil Southeast", 198L },
				{ "Canada Central", 104L },
				{ "Canada East", 114L },
				{ "Central India", 112L },
				{ "Central US", 112L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 182L },
				{ "East US", 92L },
				{ "East US 2", 94L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 14L },
				{ "France South", 10L },
				{ "Germany North", 16L },
				{ "Germany West Central", 6L },
				{ "Indonesia Central", 148L },
				{ "Israel Central", 106L },
				{ "Italy North", 100L },
				{ "Japan East", 216L },
				{ "Japan West", 216L },
				{ "Jio India Central", 112L },
				{ "Jio India West", 112L },
				{ "Korea Central", 210L },
				{ "Korea South", 204L },
				{ "Malaysia South", 148L },
				{ "Mexico Central", 154L },
				{ "New Zealand North", 240L },
				{ "North Central US", 110L },
				{ "North Europe", 31L },
				{ "Norway East", 32L },
				{ "Norway West", 26L },
				{ "Poland Central", 12L },
				{ "Qatar Central", 106L },
				{ "South Africa North", 172L },
				{ "South Africa West", 162L },
				{ "South Central US", 118L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 148L },
				{ "South India", 126L },
				{ "Spain Central", 14L },
				{ "Sweden Central", 32L },
				{ "Sweden South", 255L },
				{ "Switzerland North", 100L },
				{ "Switzerland West", 4L },
				{ "Taiwan North", 182L },
				{ "Taiwan Northwest", 182L },
				{ "UAE Central", 106L },
				{ "UAE North", 108L },
				{ "UK South", 20L },
				{ "UK West", 22L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 134L },
				{ "West Europe", 12L },
				{ "West India", 116L },
				{ "West US", 150L },
				{ "West US 2", 154L },
				{ "West US 3", 154L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Brazil South",
			new Dictionary<string, long>
			{
				{ "Australia Central", 314L },
				{ "Australia Central 2", 313L },
				{ "Australia East", 308L },
				{ "Australia Southeast", 320L },
				{ "Austria East", 198L },
				{ "Brazil South", 0L },
				{ "Brazil Southeast", 10L },
				{ "Canada Central", 132L },
				{ "Canada East", 141L },
				{ "Central India", 302L },
				{ "Central US", 146L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 320L },
				{ "East US", 118L },
				{ "East US 2", 122L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 186L },
				{ "France South", 200L },
				{ "Germany North", 198L },
				{ "Germany West Central", 192L },
				{ "Indonesia Central", 328L },
				{ "Israel Central", 296L },
				{ "Italy North", 198L },
				{ "Japan East", 262L },
				{ "Japan West", 270L },
				{ "Jio India Central", 302L },
				{ "Jio India West", 302L },
				{ "Korea Central", 300L },
				{ "Korea South", 306L },
				{ "Malaysia South", 328L },
				{ "Mexico Central", 182L },
				{ "New Zealand North", 314L },
				{ "North Central US", 138L },
				{ "North Europe", 177L },
				{ "Norway East", 208L },
				{ "Norway West", 201L },
				{ "Poland Central", 188L },
				{ "Qatar Central", 296L },
				{ "South Africa North", 342L },
				{ "South Africa West", 326L },
				{ "South Central US", 140L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 328L },
				{ "South India", 314L },
				{ "Spain Central", 186L },
				{ "Sweden Central", 208L },
				{ "Sweden South", 208L },
				{ "Switzerland North", 198L },
				{ "Switzerland West", 202L },
				{ "Taiwan North", 320L },
				{ "Taiwan Northwest", 320L },
				{ "UAE Central", 296L },
				{ "UAE North", 298L },
				{ "UK South", 181L },
				{ "UK West", 184L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 160L },
				{ "West Europe", 188L },
				{ "West India", 304L },
				{ "West US", 172L },
				{ "West US 2", 182L },
				{ "West US 3", 182L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Brazil Southeast",
			new Dictionary<string, long>
			{
				{ "Australia Central", 314L },
				{ "Australia Central 2", 313L },
				{ "Australia East", 308L },
				{ "Australia Southeast", 320L },
				{ "Austria East", 198L },
				{ "Brazil South", 10L },
				{ "Brazil Southeast", 0L },
				{ "Canada Central", 132L },
				{ "Canada East", 141L },
				{ "Central India", 302L },
				{ "Central US", 146L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 320L },
				{ "East US", 118L },
				{ "East US 2", 122L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 186L },
				{ "France South", 200L },
				{ "Germany North", 198L },
				{ "Germany West Central", 192L },
				{ "Indonesia Central", 328L },
				{ "Israel Central", 296L },
				{ "Italy North", 198L },
				{ "Japan East", 262L },
				{ "Japan West", 270L },
				{ "Jio India Central", 302L },
				{ "Jio India West", 302L },
				{ "Korea Central", 300L },
				{ "Korea South", 306L },
				{ "Malaysia South", 328L },
				{ "Mexico Central", 182L },
				{ "New Zealand North", 314L },
				{ "North Central US", 138L },
				{ "North Europe", 177L },
				{ "Norway East", 208L },
				{ "Norway West", 201L },
				{ "Poland Central", 188L },
				{ "Qatar Central", 296L },
				{ "South Africa North", 342L },
				{ "South Africa West", 326L },
				{ "South Central US", 140L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 328L },
				{ "South India", 314L },
				{ "Spain Central", 186L },
				{ "Sweden Central", 208L },
				{ "Sweden South", 208L },
				{ "Switzerland North", 198L },
				{ "Switzerland West", 202L },
				{ "Taiwan North", 320L },
				{ "Taiwan Northwest", 320L },
				{ "UAE Central", 296L },
				{ "UAE North", 298L },
				{ "UK South", 181L },
				{ "UK West", 184L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 160L },
				{ "West Europe", 188L },
				{ "West India", 304L },
				{ "West US", 172L },
				{ "West US 2", 172L },
				{ "West US 3", 182L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Canada Central",
			new Dictionary<string, long>
			{
				{ "Australia Central", 204L },
				{ "Australia Central 2", 204L },
				{ "Australia East", 200L },
				{ "Australia Southeast", 210L },
				{ "Austria East", 104L },
				{ "Brazil South", 132L },
				{ "Brazil Southeast", 132L },
				{ "Canada Central", 0L },
				{ "Canada East", 12L },
				{ "Central India", 208L },
				{ "Central US", 21L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 196L },
				{ "East US", 25L },
				{ "East US 2", 28L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 92L },
				{ "France South", 106L },
				{ "Germany North", 104L },
				{ "Germany West Central", 98L },
				{ "Indonesia Central", 216L },
				{ "Israel Central", 201L },
				{ "Italy North", 104L },
				{ "Japan East", 152L },
				{ "Japan West", 160L },
				{ "Jio India Central", 208L },
				{ "Jio India West", 208L },
				{ "Korea Central", 178L },
				{ "Korea South", 182L },
				{ "Malaysia South", 216L },
				{ "Mexico Central", 57L },
				{ "New Zealand North", 204L },
				{ "North Central US", 14L },
				{ "North Europe", 84L },
				{ "Norway East", 114L },
				{ "Norway West", 108L },
				{ "Poland Central", 94L },
				{ "Qatar Central", 201L },
				{ "South Africa North", 246L },
				{ "South Africa West", 230L },
				{ "South Central US", 42L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 216L },
				{ "South India", 220L },
				{ "Spain Central", 92L },
				{ "Sweden Central", 114L },
				{ "Sweden South", 114L },
				{ "Switzerland North", 104L },
				{ "Switzerland West", 108L },
				{ "Taiwan North", 196L },
				{ "Taiwan Northwest", 196L },
				{ "UAE Central", 201L },
				{ "UAE North", 204L },
				{ "UK South", 86L },
				{ "UK West", 90L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 36L },
				{ "West Europe", 94L },
				{ "West India", 210L },
				{ "West US", 64L },
				{ "West US 2", 57L },
				{ "West US 3", 57L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Canada East",
			new Dictionary<string, long>
			{
				{ "Australia Central", 214L },
				{ "Australia Central 2", 214L },
				{ "Australia East", 210L },
				{ "Australia Southeast", 220L },
				{ "Austria East", 114L },
				{ "Brazil South", 141L },
				{ "Brazil Southeast", 141L },
				{ "Canada Central", 12L },
				{ "Canada East", 0L },
				{ "Central India", 218L },
				{ "Central US", 30L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 206L },
				{ "East US", 34L },
				{ "East US 2", 38L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 102L },
				{ "France South", 116L },
				{ "Germany North", 114L },
				{ "Germany West Central", 108L },
				{ "Indonesia Central", 224L },
				{ "Israel Central", 210L },
				{ "Italy North", 114L },
				{ "Japan East", 162L },
				{ "Japan West", 170L },
				{ "Jio India Central", 218L },
				{ "Jio India West", 218L },
				{ "Korea Central", 186L },
				{ "Korea South", 192L },
				{ "Malaysia South", 224L },
				{ "Mexico Central", 66L },
				{ "New Zealand North", 214L },
				{ "North Central US", 24L },
				{ "North Europe", 93L },
				{ "Norway East", 124L },
				{ "Norway West", 116L },
				{ "Poland Central", 103L },
				{ "Qatar Central", 210L },
				{ "South Africa North", 256L },
				{ "South Africa West", 238L },
				{ "South Central US", 52L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 224L },
				{ "South India", 230L },
				{ "Spain Central", 102L },
				{ "Sweden Central", 124L },
				{ "Sweden South", 124L },
				{ "Switzerland North", 114L },
				{ "Switzerland West", 118L },
				{ "Taiwan North", 206L },
				{ "Taiwan Northwest", 206L },
				{ "UAE Central", 210L },
				{ "UAE North", 214L },
				{ "UK South", 96L },
				{ "UK West", 98L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 44L },
				{ "West Europe", 103L },
				{ "West India", 220L },
				{ "West US", 72L },
				{ "West US 2", 66L },
				{ "West US 3", 66L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Central India",
			new Dictionary<string, long>
			{
				{ "Australia Central", 144L },
				{ "Australia Central 2", 144L },
				{ "Australia East", 140L },
				{ "Australia Southeast", 136L },
				{ "Austria East", 112L },
				{ "Brazil South", 302L },
				{ "Brazil Southeast", 302L },
				{ "Canada Central", 208L },
				{ "Canada East", 218L },
				{ "Central India", 0L },
				{ "Central US", 222L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 86L },
				{ "East US", 196L },
				{ "East US 2", 198L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 116L },
				{ "France South", 102L },
				{ "Germany North", 128L },
				{ "Germany West Central", 118L },
				{ "Indonesia Central", 52L },
				{ "Israel Central", 30L },
				{ "Italy North", 112L },
				{ "Japan East", 120L },
				{ "Japan West", 124L },
				{ "Jio India Central", 100L },
				{ "Jio India West", 100L },
				{ "Korea Central", 114L },
				{ "Korea South", 106L },
				{ "Malaysia South", 52L },
				{ "Mexico Central", 212L },
				{ "New Zealand North", 144L },
				{ "North Central US", 216L },
				{ "North Europe", 137L },
				{ "Norway East", 146L },
				{ "Norway West", 136L },
				{ "Poland Central", 126L },
				{ "Qatar Central", 30L },
				{ "South Africa North", 264L },
				{ "South Africa West", 267L },
				{ "South Central US", 224L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 52L },
				{ "South India", 22L },
				{ "Spain Central", 116L },
				{ "Sweden Central", 146L },
				{ "Sweden South", 146L },
				{ "Switzerland North", 112L },
				{ "Switzerland West", 110L },
				{ "Taiwan North", 86L },
				{ "Taiwan Northwest", 86L },
				{ "UAE Central", 30L },
				{ "UAE North", 30L },
				{ "UK South", 122L },
				{ "UK West", 126L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 236L },
				{ "West Europe", 126L },
				{ "West India", 4L },
				{ "West US", 218L },
				{ "West US 2", 212L },
				{ "West US 3", 212L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Central US",
			new Dictionary<string, long>
			{
				{ "Australia Central", 184L },
				{ "Australia Central 2", 184L },
				{ "Australia East", 180L },
				{ "Australia Southeast", 190L },
				{ "Austria East", 112L },
				{ "Brazil South", 146L },
				{ "Brazil Southeast", 146L },
				{ "Canada Central", 21L },
				{ "Canada East", 30L },
				{ "Central India", 222L },
				{ "Central US", 0L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 176L },
				{ "East US", 23L },
				{ "East US 2", 26L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 101L },
				{ "France South", 110L },
				{ "Germany North", 118L },
				{ "Germany West Central", 112L },
				{ "Indonesia Central", 194L },
				{ "Israel Central", 216L },
				{ "Italy North", 112L },
				{ "Japan East", 140L },
				{ "Japan West", 143L },
				{ "Jio India Central", 222L },
				{ "Jio India West", 222L },
				{ "Korea Central", 158L },
				{ "Korea South", 162L },
				{ "Malaysia South", 194L },
				{ "Mexico Central", 36L },
				{ "New Zealand North", 184L },
				{ "North Central US", 8L },
				{ "North Europe", 92L },
				{ "Norway East", 128L },
				{ "Norway West", 121L },
				{ "Poland Central", 102L },
				{ "Qatar Central", 216L },
				{ "South Africa North", 260L },
				{ "South Africa West", 244L },
				{ "South Central US", 22L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 194L },
				{ "South India", 228L },
				{ "Spain Central", 101L },
				{ "Sweden Central", 128L },
				{ "Sweden South", 128L },
				{ "Switzerland North", 112L },
				{ "Switzerland West", 110L },
				{ "Taiwan North", 176L },
				{ "Taiwan Northwest", 176L },
				{ "UAE Central", 216L },
				{ "UAE North", 218L },
				{ "UK South", 96L },
				{ "UK West", 96L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 15L },
				{ "West Europe", 102L },
				{ "West India", 224L },
				{ "West US", 44L },
				{ "West US 2", 36L },
				{ "West US 3", 36L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"China East",
			new Dictionary<string, long>
			{
				{
					"Australia Central",
					long.MaxValue
				},
				{
					"Australia Central 2",
					long.MaxValue
				},
				{
					"Australia East",
					long.MaxValue
				},
				{
					"Australia Southeast",
					long.MaxValue
				},
				{
					"Austria East",
					long.MaxValue
				},
				{
					"Brazil South",
					long.MaxValue
				},
				{
					"Brazil Southeast",
					long.MaxValue
				},
				{
					"Canada Central",
					long.MaxValue
				},
				{
					"Canada East",
					long.MaxValue
				},
				{
					"Central India",
					long.MaxValue
				},
				{
					"Central US",
					long.MaxValue
				},
				{ "China East", 0L },
				{ "China East 2", 255L },
				{ "China East 3", 255L },
				{ "China North", 35L },
				{ "China North 2", 255L },
				{ "China North 3", 255L },
				{
					"East Asia",
					long.MaxValue
				},
				{
					"East US",
					long.MaxValue
				},
				{
					"East US 2",
					long.MaxValue
				},
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{
					"France Central",
					long.MaxValue
				},
				{
					"France South",
					long.MaxValue
				},
				{
					"Germany North",
					long.MaxValue
				},
				{
					"Germany West Central",
					long.MaxValue
				},
				{
					"Indonesia Central",
					long.MaxValue
				},
				{
					"Israel Central",
					long.MaxValue
				},
				{
					"Italy North",
					long.MaxValue
				},
				{
					"Japan East",
					long.MaxValue
				},
				{
					"Japan West",
					long.MaxValue
				},
				{
					"Jio India Central",
					long.MaxValue
				},
				{
					"Jio India West",
					long.MaxValue
				},
				{
					"Korea Central",
					long.MaxValue
				},
				{
					"Korea South",
					long.MaxValue
				},
				{
					"Malaysia South",
					long.MaxValue
				},
				{
					"Mexico Central",
					long.MaxValue
				},
				{
					"New Zealand North",
					long.MaxValue
				},
				{
					"North Central US",
					long.MaxValue
				},
				{
					"North Europe",
					long.MaxValue
				},
				{
					"Norway East",
					long.MaxValue
				},
				{
					"Norway West",
					long.MaxValue
				},
				{
					"Poland Central",
					long.MaxValue
				},
				{
					"Qatar Central",
					long.MaxValue
				},
				{
					"South Africa North",
					long.MaxValue
				},
				{
					"South Africa West",
					long.MaxValue
				},
				{
					"South Central US",
					long.MaxValue
				},
				{
					"South Central US STG",
					long.MaxValue
				},
				{
					"Southeast Asia",
					long.MaxValue
				},
				{
					"South India",
					long.MaxValue
				},
				{
					"Spain Central",
					long.MaxValue
				},
				{
					"Sweden Central",
					long.MaxValue
				},
				{
					"Sweden South",
					long.MaxValue
				},
				{
					"Switzerland North",
					long.MaxValue
				},
				{
					"Switzerland West",
					long.MaxValue
				},
				{
					"Taiwan North",
					long.MaxValue
				},
				{
					"Taiwan Northwest",
					long.MaxValue
				},
				{
					"UAE Central",
					long.MaxValue
				},
				{
					"UAE North",
					long.MaxValue
				},
				{
					"UK South",
					long.MaxValue
				},
				{
					"UK West",
					long.MaxValue
				},
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{
					"West Central US",
					long.MaxValue
				},
				{
					"West Europe",
					long.MaxValue
				},
				{
					"West India",
					long.MaxValue
				},
				{
					"West US",
					long.MaxValue
				},
				{
					"West US 2",
					long.MaxValue
				},
				{
					"West US 3",
					long.MaxValue
				},
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"China East 2",
			new Dictionary<string, long>
			{
				{
					"Australia Central",
					long.MaxValue
				},
				{
					"Australia Central 2",
					long.MaxValue
				},
				{
					"Australia East",
					long.MaxValue
				},
				{
					"Australia Southeast",
					long.MaxValue
				},
				{
					"Austria East",
					long.MaxValue
				},
				{
					"Brazil South",
					long.MaxValue
				},
				{
					"Brazil Southeast",
					long.MaxValue
				},
				{
					"Canada Central",
					long.MaxValue
				},
				{
					"Canada East",
					long.MaxValue
				},
				{
					"Central India",
					long.MaxValue
				},
				{
					"Central US",
					long.MaxValue
				},
				{ "China East", 255L },
				{ "China East 2", 0L },
				{ "China East 3", 255L },
				{ "China North", 255L },
				{ "China North 2", 35L },
				{ "China North 3", 255L },
				{
					"East Asia",
					long.MaxValue
				},
				{
					"East US",
					long.MaxValue
				},
				{
					"East US 2",
					long.MaxValue
				},
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{
					"France Central",
					long.MaxValue
				},
				{
					"France South",
					long.MaxValue
				},
				{
					"Germany North",
					long.MaxValue
				},
				{
					"Germany West Central",
					long.MaxValue
				},
				{
					"Indonesia Central",
					long.MaxValue
				},
				{
					"Israel Central",
					long.MaxValue
				},
				{
					"Italy North",
					long.MaxValue
				},
				{
					"Japan East",
					long.MaxValue
				},
				{
					"Japan West",
					long.MaxValue
				},
				{
					"Jio India Central",
					long.MaxValue
				},
				{
					"Jio India West",
					long.MaxValue
				},
				{
					"Korea Central",
					long.MaxValue
				},
				{
					"Korea South",
					long.MaxValue
				},
				{
					"Malaysia South",
					long.MaxValue
				},
				{
					"Mexico Central",
					long.MaxValue
				},
				{
					"New Zealand North",
					long.MaxValue
				},
				{
					"North Central US",
					long.MaxValue
				},
				{
					"North Europe",
					long.MaxValue
				},
				{
					"Norway East",
					long.MaxValue
				},
				{
					"Norway West",
					long.MaxValue
				},
				{
					"Poland Central",
					long.MaxValue
				},
				{
					"Qatar Central",
					long.MaxValue
				},
				{
					"South Africa North",
					long.MaxValue
				},
				{
					"South Africa West",
					long.MaxValue
				},
				{
					"South Central US",
					long.MaxValue
				},
				{
					"South Central US STG",
					long.MaxValue
				},
				{
					"Southeast Asia",
					long.MaxValue
				},
				{
					"South India",
					long.MaxValue
				},
				{
					"Spain Central",
					long.MaxValue
				},
				{
					"Sweden Central",
					long.MaxValue
				},
				{
					"Sweden South",
					long.MaxValue
				},
				{
					"Switzerland North",
					long.MaxValue
				},
				{
					"Switzerland West",
					long.MaxValue
				},
				{
					"Taiwan North",
					long.MaxValue
				},
				{
					"Taiwan Northwest",
					long.MaxValue
				},
				{
					"UAE Central",
					long.MaxValue
				},
				{
					"UAE North",
					long.MaxValue
				},
				{
					"UK South",
					long.MaxValue
				},
				{
					"UK West",
					long.MaxValue
				},
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{
					"West Central US",
					long.MaxValue
				},
				{
					"West Europe",
					long.MaxValue
				},
				{
					"West India",
					long.MaxValue
				},
				{
					"West US",
					long.MaxValue
				},
				{
					"West US 2",
					long.MaxValue
				},
				{
					"West US 3",
					long.MaxValue
				},
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"China East 3",
			new Dictionary<string, long>
			{
				{
					"Australia Central",
					long.MaxValue
				},
				{
					"Australia Central 2",
					long.MaxValue
				},
				{
					"Australia East",
					long.MaxValue
				},
				{
					"Australia Southeast",
					long.MaxValue
				},
				{
					"Austria East",
					long.MaxValue
				},
				{
					"Brazil South",
					long.MaxValue
				},
				{
					"Brazil Southeast",
					long.MaxValue
				},
				{
					"Canada Central",
					long.MaxValue
				},
				{
					"Canada East",
					long.MaxValue
				},
				{
					"Central India",
					long.MaxValue
				},
				{
					"Central US",
					long.MaxValue
				},
				{ "China East", 255L },
				{ "China East 2", 255L },
				{ "China East 3", 0L },
				{ "China North", 255L },
				{ "China North 2", 255L },
				{ "China North 3", 100L },
				{
					"East Asia",
					long.MaxValue
				},
				{
					"East US",
					long.MaxValue
				},
				{
					"East US 2",
					long.MaxValue
				},
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{
					"France Central",
					long.MaxValue
				},
				{
					"France South",
					long.MaxValue
				},
				{
					"Germany North",
					long.MaxValue
				},
				{
					"Germany West Central",
					long.MaxValue
				},
				{
					"Indonesia Central",
					long.MaxValue
				},
				{
					"Israel Central",
					long.MaxValue
				},
				{
					"Italy North",
					long.MaxValue
				},
				{
					"Japan East",
					long.MaxValue
				},
				{
					"Japan West",
					long.MaxValue
				},
				{
					"Jio India Central",
					long.MaxValue
				},
				{
					"Jio India West",
					long.MaxValue
				},
				{
					"Korea Central",
					long.MaxValue
				},
				{
					"Korea South",
					long.MaxValue
				},
				{
					"Malaysia South",
					long.MaxValue
				},
				{
					"Mexico Central",
					long.MaxValue
				},
				{
					"New Zealand North",
					long.MaxValue
				},
				{
					"North Central US",
					long.MaxValue
				},
				{
					"North Europe",
					long.MaxValue
				},
				{
					"Norway East",
					long.MaxValue
				},
				{
					"Norway West",
					long.MaxValue
				},
				{
					"Poland Central",
					long.MaxValue
				},
				{
					"Qatar Central",
					long.MaxValue
				},
				{
					"South Africa North",
					long.MaxValue
				},
				{
					"South Africa West",
					long.MaxValue
				},
				{
					"South Central US",
					long.MaxValue
				},
				{
					"South Central US STG",
					long.MaxValue
				},
				{
					"Southeast Asia",
					long.MaxValue
				},
				{
					"South India",
					long.MaxValue
				},
				{
					"Spain Central",
					long.MaxValue
				},
				{
					"Sweden Central",
					long.MaxValue
				},
				{
					"Sweden South",
					long.MaxValue
				},
				{
					"Switzerland North",
					long.MaxValue
				},
				{
					"Switzerland West",
					long.MaxValue
				},
				{
					"Taiwan North",
					long.MaxValue
				},
				{
					"Taiwan Northwest",
					long.MaxValue
				},
				{
					"UAE Central",
					long.MaxValue
				},
				{
					"UAE North",
					long.MaxValue
				},
				{
					"UK South",
					long.MaxValue
				},
				{
					"UK West",
					long.MaxValue
				},
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{
					"West Central US",
					long.MaxValue
				},
				{
					"West Europe",
					long.MaxValue
				},
				{
					"West India",
					long.MaxValue
				},
				{
					"West US",
					long.MaxValue
				},
				{
					"West US 2",
					long.MaxValue
				},
				{
					"West US 3",
					long.MaxValue
				},
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"China North",
			new Dictionary<string, long>
			{
				{
					"Australia Central",
					long.MaxValue
				},
				{
					"Australia Central 2",
					long.MaxValue
				},
				{
					"Australia East",
					long.MaxValue
				},
				{
					"Australia Southeast",
					long.MaxValue
				},
				{
					"Austria East",
					long.MaxValue
				},
				{
					"Brazil South",
					long.MaxValue
				},
				{
					"Brazil Southeast",
					long.MaxValue
				},
				{
					"Canada Central",
					long.MaxValue
				},
				{
					"Canada East",
					long.MaxValue
				},
				{
					"Central India",
					long.MaxValue
				},
				{
					"Central US",
					long.MaxValue
				},
				{ "China East", 35L },
				{ "China East 2", 255L },
				{ "China East 3", 255L },
				{ "China North", 0L },
				{ "China North 2", 255L },
				{ "China North 3", 255L },
				{
					"East Asia",
					long.MaxValue
				},
				{
					"East US",
					long.MaxValue
				},
				{
					"East US 2",
					long.MaxValue
				},
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{
					"France Central",
					long.MaxValue
				},
				{
					"France South",
					long.MaxValue
				},
				{
					"Germany North",
					long.MaxValue
				},
				{
					"Germany West Central",
					long.MaxValue
				},
				{
					"Indonesia Central",
					long.MaxValue
				},
				{
					"Israel Central",
					long.MaxValue
				},
				{
					"Italy North",
					long.MaxValue
				},
				{
					"Japan East",
					long.MaxValue
				},
				{
					"Japan West",
					long.MaxValue
				},
				{
					"Jio India Central",
					long.MaxValue
				},
				{
					"Jio India West",
					long.MaxValue
				},
				{
					"Korea Central",
					long.MaxValue
				},
				{
					"Korea South",
					long.MaxValue
				},
				{
					"Malaysia South",
					long.MaxValue
				},
				{
					"Mexico Central",
					long.MaxValue
				},
				{
					"New Zealand North",
					long.MaxValue
				},
				{
					"North Central US",
					long.MaxValue
				},
				{
					"North Europe",
					long.MaxValue
				},
				{
					"Norway East",
					long.MaxValue
				},
				{
					"Norway West",
					long.MaxValue
				},
				{
					"Poland Central",
					long.MaxValue
				},
				{
					"Qatar Central",
					long.MaxValue
				},
				{
					"South Africa North",
					long.MaxValue
				},
				{
					"South Africa West",
					long.MaxValue
				},
				{
					"South Central US",
					long.MaxValue
				},
				{
					"South Central US STG",
					long.MaxValue
				},
				{
					"Southeast Asia",
					long.MaxValue
				},
				{
					"South India",
					long.MaxValue
				},
				{
					"Spain Central",
					long.MaxValue
				},
				{
					"Sweden Central",
					long.MaxValue
				},
				{
					"Sweden South",
					long.MaxValue
				},
				{
					"Switzerland North",
					long.MaxValue
				},
				{
					"Switzerland West",
					long.MaxValue
				},
				{
					"Taiwan North",
					long.MaxValue
				},
				{
					"Taiwan Northwest",
					long.MaxValue
				},
				{
					"UAE Central",
					long.MaxValue
				},
				{
					"UAE North",
					long.MaxValue
				},
				{
					"UK South",
					long.MaxValue
				},
				{
					"UK West",
					long.MaxValue
				},
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{
					"West Central US",
					long.MaxValue
				},
				{
					"West Europe",
					long.MaxValue
				},
				{
					"West India",
					long.MaxValue
				},
				{
					"West US",
					long.MaxValue
				},
				{
					"West US 2",
					long.MaxValue
				},
				{
					"West US 3",
					long.MaxValue
				},
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"China North 2",
			new Dictionary<string, long>
			{
				{
					"Australia Central",
					long.MaxValue
				},
				{
					"Australia Central 2",
					long.MaxValue
				},
				{
					"Australia East",
					long.MaxValue
				},
				{
					"Australia Southeast",
					long.MaxValue
				},
				{
					"Austria East",
					long.MaxValue
				},
				{
					"Brazil South",
					long.MaxValue
				},
				{
					"Brazil Southeast",
					long.MaxValue
				},
				{
					"Canada Central",
					long.MaxValue
				},
				{
					"Canada East",
					long.MaxValue
				},
				{
					"Central India",
					long.MaxValue
				},
				{
					"Central US",
					long.MaxValue
				},
				{ "China East", 255L },
				{ "China East 2", 35L },
				{ "China East 3", 255L },
				{ "China North", 255L },
				{ "China North 2", 0L },
				{ "China North 3", 255L },
				{
					"East Asia",
					long.MaxValue
				},
				{
					"East US",
					long.MaxValue
				},
				{
					"East US 2",
					long.MaxValue
				},
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{
					"France Central",
					long.MaxValue
				},
				{
					"France South",
					long.MaxValue
				},
				{
					"Germany North",
					long.MaxValue
				},
				{
					"Germany West Central",
					long.MaxValue
				},
				{
					"Indonesia Central",
					long.MaxValue
				},
				{
					"Israel Central",
					long.MaxValue
				},
				{
					"Italy North",
					long.MaxValue
				},
				{
					"Japan East",
					long.MaxValue
				},
				{
					"Japan West",
					long.MaxValue
				},
				{
					"Jio India Central",
					long.MaxValue
				},
				{
					"Jio India West",
					long.MaxValue
				},
				{
					"Korea Central",
					long.MaxValue
				},
				{
					"Korea South",
					long.MaxValue
				},
				{
					"Malaysia South",
					long.MaxValue
				},
				{
					"Mexico Central",
					long.MaxValue
				},
				{
					"New Zealand North",
					long.MaxValue
				},
				{
					"North Central US",
					long.MaxValue
				},
				{
					"North Europe",
					long.MaxValue
				},
				{
					"Norway East",
					long.MaxValue
				},
				{
					"Norway West",
					long.MaxValue
				},
				{
					"Poland Central",
					long.MaxValue
				},
				{
					"Qatar Central",
					long.MaxValue
				},
				{
					"South Africa North",
					long.MaxValue
				},
				{
					"South Africa West",
					long.MaxValue
				},
				{
					"South Central US",
					long.MaxValue
				},
				{
					"South Central US STG",
					long.MaxValue
				},
				{
					"Southeast Asia",
					long.MaxValue
				},
				{
					"South India",
					long.MaxValue
				},
				{
					"Spain Central",
					long.MaxValue
				},
				{
					"Sweden Central",
					long.MaxValue
				},
				{
					"Sweden South",
					long.MaxValue
				},
				{
					"Switzerland North",
					long.MaxValue
				},
				{
					"Switzerland West",
					long.MaxValue
				},
				{
					"Taiwan North",
					long.MaxValue
				},
				{
					"Taiwan Northwest",
					long.MaxValue
				},
				{
					"UAE Central",
					long.MaxValue
				},
				{
					"UAE North",
					long.MaxValue
				},
				{
					"UK South",
					long.MaxValue
				},
				{
					"UK West",
					long.MaxValue
				},
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{
					"West Central US",
					long.MaxValue
				},
				{
					"West Europe",
					long.MaxValue
				},
				{
					"West India",
					long.MaxValue
				},
				{
					"West US",
					long.MaxValue
				},
				{
					"West US 2",
					long.MaxValue
				},
				{
					"West US 3",
					long.MaxValue
				},
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"China North 3",
			new Dictionary<string, long>
			{
				{
					"Australia Central",
					long.MaxValue
				},
				{
					"Australia Central 2",
					long.MaxValue
				},
				{
					"Australia East",
					long.MaxValue
				},
				{
					"Australia Southeast",
					long.MaxValue
				},
				{
					"Austria East",
					long.MaxValue
				},
				{
					"Brazil South",
					long.MaxValue
				},
				{
					"Brazil Southeast",
					long.MaxValue
				},
				{
					"Canada Central",
					long.MaxValue
				},
				{
					"Canada East",
					long.MaxValue
				},
				{
					"Central India",
					long.MaxValue
				},
				{
					"Central US",
					long.MaxValue
				},
				{ "China East", 255L },
				{ "China East 2", 255L },
				{ "China East 3", 100L },
				{ "China North", 255L },
				{ "China North 2", 255L },
				{ "China North 3", 0L },
				{
					"East Asia",
					long.MaxValue
				},
				{
					"East US",
					long.MaxValue
				},
				{
					"East US 2",
					long.MaxValue
				},
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{
					"France Central",
					long.MaxValue
				},
				{
					"France South",
					long.MaxValue
				},
				{
					"Germany North",
					long.MaxValue
				},
				{
					"Germany West Central",
					long.MaxValue
				},
				{
					"Indonesia Central",
					long.MaxValue
				},
				{
					"Israel Central",
					long.MaxValue
				},
				{
					"Italy North",
					long.MaxValue
				},
				{
					"Japan East",
					long.MaxValue
				},
				{
					"Japan West",
					long.MaxValue
				},
				{
					"Jio India Central",
					long.MaxValue
				},
				{
					"Jio India West",
					long.MaxValue
				},
				{
					"Korea Central",
					long.MaxValue
				},
				{
					"Korea South",
					long.MaxValue
				},
				{
					"Malaysia South",
					long.MaxValue
				},
				{
					"Mexico Central",
					long.MaxValue
				},
				{
					"New Zealand North",
					long.MaxValue
				},
				{
					"North Central US",
					long.MaxValue
				},
				{
					"North Europe",
					long.MaxValue
				},
				{
					"Norway East",
					long.MaxValue
				},
				{
					"Norway West",
					long.MaxValue
				},
				{
					"Poland Central",
					long.MaxValue
				},
				{
					"Qatar Central",
					long.MaxValue
				},
				{
					"South Africa North",
					long.MaxValue
				},
				{
					"South Africa West",
					long.MaxValue
				},
				{
					"South Central US",
					long.MaxValue
				},
				{
					"South Central US STG",
					long.MaxValue
				},
				{
					"Southeast Asia",
					long.MaxValue
				},
				{
					"South India",
					long.MaxValue
				},
				{
					"Spain Central",
					long.MaxValue
				},
				{
					"Sweden Central",
					long.MaxValue
				},
				{
					"Sweden South",
					long.MaxValue
				},
				{
					"Switzerland North",
					long.MaxValue
				},
				{
					"Switzerland West",
					long.MaxValue
				},
				{
					"Taiwan North",
					long.MaxValue
				},
				{
					"Taiwan Northwest",
					long.MaxValue
				},
				{
					"UAE Central",
					long.MaxValue
				},
				{
					"UAE North",
					long.MaxValue
				},
				{
					"UK South",
					long.MaxValue
				},
				{
					"UK West",
					long.MaxValue
				},
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{
					"West Central US",
					long.MaxValue
				},
				{
					"West Europe",
					long.MaxValue
				},
				{
					"West India",
					long.MaxValue
				},
				{
					"West US",
					long.MaxValue
				},
				{
					"West US 2",
					long.MaxValue
				},
				{
					"West US 3",
					long.MaxValue
				},
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"East Asia",
			new Dictionary<string, long>
			{
				{ "Australia Central", 120L },
				{ "Australia Central 2", 120L },
				{ "Australia East", 116L },
				{ "Australia Southeast", 118L },
				{ "Austria East", 182L },
				{ "Brazil South", 320L },
				{ "Brazil Southeast", 320L },
				{ "Canada Central", 196L },
				{ "Canada East", 206L },
				{ "Central India", 86L },
				{ "Central US", 176L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 0L },
				{ "East US", 199L },
				{ "East US 2", 210L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 184L },
				{ "France South", 172L },
				{ "Germany North", 196L },
				{ "Germany West Central", 188L },
				{ "Indonesia Central", 33L },
				{ "Israel Central", 110L },
				{ "Italy North", 182L },
				{ "Japan East", 50L },
				{ "Japan West", 50L },
				{ "Jio India Central", 86L },
				{ "Jio India West", 86L },
				{ "Korea Central", 56L },
				{ "Korea South", 60L },
				{ "Malaysia South", 33L },
				{ "Mexico Central", 140L },
				{ "New Zealand North", 120L },
				{ "North Central US", 184L },
				{ "North Europe", 205L },
				{ "Norway East", 214L },
				{ "Norway West", 206L },
				{ "Poland Central", 193L },
				{ "Qatar Central", 110L },
				{ "South Africa North", 332L },
				{ "South Africa West", 336L },
				{ "South Central US", 182L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 33L },
				{ "South India", 66L },
				{ "Spain Central", 184L },
				{ "Sweden Central", 214L },
				{ "Sweden South", 214L },
				{ "Switzerland North", 182L },
				{ "Switzerland West", 178L },
				{ "Taiwan North", 100L },
				{ "Taiwan Northwest", 100L },
				{ "UAE Central", 110L },
				{ "UAE North", 109L },
				{ "UK South", 192L },
				{ "UK West", 194L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 162L },
				{ "West Europe", 193L },
				{ "West India", 84L },
				{ "West US", 148L },
				{ "West US 2", 140L },
				{ "West US 3", 140L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"East US",
			new Dictionary<string, long>
			{
				{ "Australia Central", 205L },
				{ "Australia Central 2", 205L },
				{ "Australia East", 200L },
				{ "Australia Southeast", 212L },
				{ "Austria East", 92L },
				{ "Brazil South", 118L },
				{ "Brazil Southeast", 118L },
				{ "Canada Central", 25L },
				{ "Canada East", 34L },
				{ "Central India", 196L },
				{ "Central US", 23L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 199L },
				{ "East US", 0L },
				{ "East US 2", 6L },
				{
					"East US SLV",
					long.MaxValue
				},
				{ "East US STG", 0L },
				{ "France Central", 80L },
				{ "France South", 90L },
				{ "Germany North", 91L },
				{ "Germany West Central", 86L },
				{ "Indonesia Central", 218L },
				{ "Israel Central", 189L },
				{ "Italy North", 92L },
				{ "Japan East", 162L },
				{ "Japan West", 168L },
				{ "Jio India Central", 196L },
				{ "Jio India West", 196L },
				{ "Korea Central", 182L },
				{ "Korea South", 186L },
				{ "Malaysia South", 218L },
				{ "Mexico Central", 58L },
				{ "New Zealand North", 205L },
				{ "North Central US", 19L },
				{ "North Europe", 76L },
				{ "Norway East", 101L },
				{ "Norway West", 94L },
				{ "Poland Central", 81L },
				{ "Qatar Central", 189L },
				{ "South Africa North", 234L },
				{ "South Africa West", 218L },
				{ "South Central US", 32L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 218L },
				{ "South India", 208L },
				{ "Spain Central", 80L },
				{ "Sweden Central", 101L },
				{ "Sweden South", 101L },
				{ "Switzerland North", 92L },
				{ "Switzerland West", 88L },
				{ "Taiwan North", 199L },
				{ "Taiwan Northwest", 199L },
				{ "UAE Central", 189L },
				{ "UAE North", 192L },
				{ "UK South", 74L },
				{ "UK West", 76L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 38L },
				{ "West Europe", 81L },
				{ "West India", 198L },
				{ "West US", 64L },
				{ "West US 2", 58L },
				{ "West US 3", 58L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"East US 2",
			new Dictionary<string, long>
			{
				{ "Australia Central", 200L },
				{ "Australia Central 2", 200L },
				{ "Australia East", 196L },
				{ "Australia Southeast", 206L },
				{ "Austria East", 94L },
				{ "Brazil South", 122L },
				{ "Brazil Southeast", 122L },
				{ "Canada Central", 28L },
				{ "Canada East", 38L },
				{ "Central India", 198L },
				{ "Central US", 26L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 210L },
				{ "East US", 6L },
				{ "East US 2", 0L },
				{
					"East US SLV",
					long.MaxValue
				},
				{ "East US STG", 6L },
				{ "France Central", 80L },
				{ "France South", 90L },
				{ "Germany North", 94L },
				{ "Germany West Central", 90L },
				{ "Indonesia Central", 226L },
				{ "Israel Central", 190L },
				{ "Italy North", 94L },
				{ "Japan East", 164L },
				{ "Japan West", 164L },
				{ "Jio India Central", 198L },
				{ "Jio India West", 198L },
				{ "Korea Central", 176L },
				{ "Korea South", 182L },
				{ "Malaysia South", 226L },
				{ "Mexico Central", 68L },
				{ "New Zealand North", 200L },
				{ "North Central US", 22L },
				{ "North Europe", 75L },
				{ "Norway East", 104L },
				{ "Norway West", 98L },
				{ "Poland Central", 86L },
				{ "Qatar Central", 190L },
				{ "South Africa North", 240L },
				{ "South Africa West", 222L },
				{ "South Central US", 26L },
				{ "South Central US STG", 26L },
				{ "Southeast Asia", 226L },
				{ "South India", 210L },
				{ "Spain Central", 80L },
				{ "Sweden Central", 104L },
				{ "Sweden South", 104L },
				{ "Switzerland North", 94L },
				{ "Switzerland West", 88L },
				{ "Taiwan North", 210L },
				{ "Taiwan Northwest", 210L },
				{ "UAE Central", 190L },
				{ "UAE North", 194L },
				{ "UK South", 78L },
				{ "UK West", 80L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 47L },
				{ "West Europe", 86L },
				{ "West India", 200L },
				{ "West US", 58L },
				{ "West US 2", 68L },
				{ "West US 3", 68L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"East US SLV",
			new Dictionary<string, long>
			{
				{
					"Australia Central",
					long.MaxValue
				},
				{
					"Australia Central 2",
					long.MaxValue
				},
				{
					"Australia East",
					long.MaxValue
				},
				{
					"Australia Southeast",
					long.MaxValue
				},
				{
					"Austria East",
					long.MaxValue
				},
				{
					"Brazil South",
					long.MaxValue
				},
				{
					"Brazil Southeast",
					long.MaxValue
				},
				{
					"Canada Central",
					long.MaxValue
				},
				{
					"Canada East",
					long.MaxValue
				},
				{
					"Central India",
					long.MaxValue
				},
				{
					"Central US",
					long.MaxValue
				},
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{
					"East Asia",
					long.MaxValue
				},
				{
					"East US",
					long.MaxValue
				},
				{
					"East US 2",
					long.MaxValue
				},
				{ "East US SLV", 0L },
				{
					"East US STG",
					long.MaxValue
				},
				{
					"France Central",
					long.MaxValue
				},
				{
					"France South",
					long.MaxValue
				},
				{
					"Germany North",
					long.MaxValue
				},
				{
					"Germany West Central",
					long.MaxValue
				},
				{
					"Indonesia Central",
					long.MaxValue
				},
				{
					"Israel Central",
					long.MaxValue
				},
				{
					"Italy North",
					long.MaxValue
				},
				{
					"Japan East",
					long.MaxValue
				},
				{
					"Japan West",
					long.MaxValue
				},
				{
					"Jio India Central",
					long.MaxValue
				},
				{
					"Jio India West",
					long.MaxValue
				},
				{
					"Korea Central",
					long.MaxValue
				},
				{
					"Korea South",
					long.MaxValue
				},
				{
					"Malaysia South",
					long.MaxValue
				},
				{
					"Mexico Central",
					long.MaxValue
				},
				{
					"New Zealand North",
					long.MaxValue
				},
				{
					"North Central US",
					long.MaxValue
				},
				{
					"North Europe",
					long.MaxValue
				},
				{
					"Norway East",
					long.MaxValue
				},
				{
					"Norway West",
					long.MaxValue
				},
				{
					"Poland Central",
					long.MaxValue
				},
				{
					"Qatar Central",
					long.MaxValue
				},
				{
					"South Africa North",
					long.MaxValue
				},
				{
					"South Africa West",
					long.MaxValue
				},
				{
					"South Central US",
					long.MaxValue
				},
				{
					"South Central US STG",
					long.MaxValue
				},
				{
					"Southeast Asia",
					long.MaxValue
				},
				{
					"South India",
					long.MaxValue
				},
				{
					"Spain Central",
					long.MaxValue
				},
				{ "Sweden Central", 255L },
				{ "Sweden South", 255L },
				{
					"Switzerland North",
					long.MaxValue
				},
				{
					"Switzerland West",
					long.MaxValue
				},
				{
					"Taiwan North",
					long.MaxValue
				},
				{
					"Taiwan Northwest",
					long.MaxValue
				},
				{
					"UAE Central",
					long.MaxValue
				},
				{
					"UAE North",
					long.MaxValue
				},
				{
					"UK South",
					long.MaxValue
				},
				{
					"UK West",
					long.MaxValue
				},
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{
					"West Central US",
					long.MaxValue
				},
				{
					"West Europe",
					long.MaxValue
				},
				{
					"West India",
					long.MaxValue
				},
				{
					"West US",
					long.MaxValue
				},
				{
					"West US 2",
					long.MaxValue
				},
				{
					"West US 3",
					long.MaxValue
				},
				{ "Central US EUAP", 255L },
				{ "East US 2 EUAP", 255L }
			}
		},
		{
			"East US STG",
			new Dictionary<string, long>
			{
				{
					"Australia Central",
					long.MaxValue
				},
				{
					"Australia Central 2",
					long.MaxValue
				},
				{
					"Australia East",
					long.MaxValue
				},
				{
					"Australia Southeast",
					long.MaxValue
				},
				{
					"Austria East",
					long.MaxValue
				},
				{
					"Brazil South",
					long.MaxValue
				},
				{
					"Brazil Southeast",
					long.MaxValue
				},
				{
					"Canada Central",
					long.MaxValue
				},
				{
					"Canada East",
					long.MaxValue
				},
				{
					"Central India",
					long.MaxValue
				},
				{
					"Central US",
					long.MaxValue
				},
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{
					"East Asia",
					long.MaxValue
				},
				{ "East US", 0L },
				{ "East US 2", 6L },
				{
					"East US SLV",
					long.MaxValue
				},
				{ "East US STG", 0L },
				{
					"France Central",
					long.MaxValue
				},
				{
					"France South",
					long.MaxValue
				},
				{
					"Germany North",
					long.MaxValue
				},
				{
					"Germany West Central",
					long.MaxValue
				},
				{ "Indonesia Central", 218L },
				{
					"Israel Central",
					long.MaxValue
				},
				{
					"Italy North",
					long.MaxValue
				},
				{
					"Japan East",
					long.MaxValue
				},
				{
					"Japan West",
					long.MaxValue
				},
				{
					"Jio India Central",
					long.MaxValue
				},
				{
					"Jio India West",
					long.MaxValue
				},
				{
					"Korea Central",
					long.MaxValue
				},
				{
					"Korea South",
					long.MaxValue
				},
				{
					"Malaysia South",
					long.MaxValue
				},
				{
					"Mexico Central",
					long.MaxValue
				},
				{
					"New Zealand North",
					long.MaxValue
				},
				{ "North Central US", 19L },
				{
					"North Europe",
					long.MaxValue
				},
				{
					"Norway East",
					long.MaxValue
				},
				{
					"Norway West",
					long.MaxValue
				},
				{
					"Poland Central",
					long.MaxValue
				},
				{
					"Qatar Central",
					long.MaxValue
				},
				{
					"South Africa North",
					long.MaxValue
				},
				{
					"South Africa West",
					long.MaxValue
				},
				{
					"South Central US",
					long.MaxValue
				},
				{ "South Central US STG", 32L },
				{ "Southeast Asia", 218L },
				{
					"South India",
					long.MaxValue
				},
				{
					"Spain Central",
					long.MaxValue
				},
				{
					"Sweden Central",
					long.MaxValue
				},
				{
					"Sweden South",
					long.MaxValue
				},
				{
					"Switzerland North",
					long.MaxValue
				},
				{
					"Switzerland West",
					long.MaxValue
				},
				{
					"Taiwan North",
					long.MaxValue
				},
				{
					"Taiwan Northwest",
					long.MaxValue
				},
				{
					"UAE Central",
					long.MaxValue
				},
				{
					"UAE North",
					long.MaxValue
				},
				{
					"UK South",
					long.MaxValue
				},
				{
					"UK West",
					long.MaxValue
				},
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{
					"West Central US",
					long.MaxValue
				},
				{
					"West Europe",
					long.MaxValue
				},
				{
					"West India",
					long.MaxValue
				},
				{
					"West US",
					long.MaxValue
				},
				{ "West US 2", 58L },
				{
					"West US 3",
					long.MaxValue
				},
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"France Central",
			new Dictionary<string, long>
			{
				{ "Australia Central", 242L },
				{ "Australia Central 2", 242L },
				{ "Australia East", 238L },
				{ "Australia Southeast", 235L },
				{ "Austria East", 14L },
				{ "Brazil South", 186L },
				{ "Brazil Southeast", 186L },
				{ "Canada Central", 92L },
				{ "Canada East", 102L },
				{ "Central India", 116L },
				{ "Central US", 101L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 184L },
				{ "East US", 80L },
				{ "East US 2", 80L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 0L },
				{ "France South", 11L },
				{ "Germany North", 20L },
				{ "Germany West Central", 10L },
				{ "Indonesia Central", 150L },
				{ "Israel Central", 108L },
				{ "Italy North", 14L },
				{ "Japan East", 218L },
				{ "Japan West", 218L },
				{ "Jio India Central", 116L },
				{ "Jio India West", 116L },
				{ "Korea Central", 212L },
				{ "Korea South", 206L },
				{ "Malaysia South", 150L },
				{ "Mexico Central", 140L },
				{ "New Zealand North", 242L },
				{ "North Central US", 96L },
				{ "North Europe", 27L },
				{ "Norway East", 30L },
				{ "Norway West", 24L },
				{ "Poland Central", 10L },
				{ "Qatar Central", 108L },
				{ "South Africa North", 166L },
				{ "South Africa West", 150L },
				{ "South Central US", 106L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 150L },
				{ "South India", 128L },
				{ "Spain Central", 100L },
				{ "Sweden Central", 30L },
				{ "Sweden South", 30L },
				{ "Switzerland North", 14L },
				{ "Switzerland West", 10L },
				{ "Taiwan North", 184L },
				{ "Taiwan Northwest", 184L },
				{ "UAE Central", 108L },
				{ "UAE North", 112L },
				{ "UK South", 7L },
				{ "UK West", 8L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 119L },
				{ "West Europe", 10L },
				{ "West India", 118L },
				{ "West US", 138L },
				{ "West US 2", 140L },
				{ "West US 3", 140L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"France South",
			new Dictionary<string, long>
			{
				{ "Australia Central", 230L },
				{ "Australia Central 2", 230L },
				{ "Australia East", 224L },
				{ "Australia Southeast", 222L },
				{ "Austria East", 10L },
				{ "Brazil South", 200L },
				{ "Brazil Southeast", 200L },
				{ "Canada Central", 106L },
				{ "Canada East", 116L },
				{ "Central India", 102L },
				{ "Central US", 110L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 172L },
				{ "East US", 90L },
				{ "East US 2", 90L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 11L },
				{ "France South", 0L },
				{ "Germany North", 26L },
				{ "Germany West Central", 17L },
				{ "Indonesia Central", 138L },
				{ "Israel Central", 96L },
				{ "Italy North", 10L },
				{ "Japan East", 204L },
				{ "Japan West", 206L },
				{ "Jio India Central", 102L },
				{ "Jio India West", 102L },
				{ "Korea Central", 200L },
				{ "Korea South", 193L },
				{ "Malaysia South", 138L },
				{ "Mexico Central", 146L },
				{ "New Zealand North", 230L },
				{ "North Central US", 104L },
				{ "North Europe", 31L },
				{ "Norway East", 42L },
				{ "Norway West", 36L },
				{ "Poland Central", 20L },
				{ "Qatar Central", 96L },
				{ "South Africa North", 162L },
				{ "South Africa West", 166L },
				{ "South Central US", 116L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 138L },
				{ "South India", 114L },
				{ "Spain Central", 11L },
				{ "Sweden Central", 42L },
				{ "Sweden South", 42L },
				{ "Switzerland North", 10L },
				{ "Switzerland West", 8L },
				{ "Taiwan North", 172L },
				{ "Taiwan Northwest", 172L },
				{ "UAE Central", 96L },
				{ "UAE North", 98L },
				{ "UK South", 16L },
				{ "UK West", 18L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 126L },
				{ "West Europe", 20L },
				{ "West India", 106L },
				{ "West US", 148L },
				{ "West US 2", 146L },
				{ "West US 3", 146L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Germany North",
			new Dictionary<string, long>
			{
				{ "Australia Central", 254L },
				{ "Australia Central 2", 256L },
				{ "Australia East", 250L },
				{ "Australia Southeast", 246L },
				{ "Austria East", 16L },
				{ "Brazil South", 198L },
				{ "Brazil Southeast", 198L },
				{ "Canada Central", 104L },
				{ "Canada East", 114L },
				{ "Central India", 128L },
				{ "Central US", 118L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 196L },
				{ "East US", 91L },
				{ "East US 2", 94L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 20L },
				{ "France South", 26L },
				{ "Germany North", 0L },
				{ "Germany West Central", 10L },
				{ "Indonesia Central", 162L },
				{ "Israel Central", 120L },
				{ "Italy North", 16L },
				{ "Japan East", 230L },
				{ "Japan West", 230L },
				{ "Jio India Central", 128L },
				{ "Jio India West", 128L },
				{ "Korea Central", 224L },
				{ "Korea South", 218L },
				{ "Malaysia South", 162L },
				{ "Mexico Central", 154L },
				{ "New Zealand North", 254L },
				{ "North Central US", 112L },
				{ "North Europe", 31L },
				{ "Norway East", 20L },
				{ "Norway West", 26L },
				{ "Poland Central", 10L },
				{ "Qatar Central", 120L },
				{ "South Africa North", 178L },
				{ "South Africa West", 162L },
				{ "South Central US", 120L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 162L },
				{ "South India", 140L },
				{ "Spain Central", 20L },
				{ "Sweden Central", 20L },
				{ "Sweden South", 20L },
				{ "Switzerland North", 16L },
				{ "Switzerland West", 20L },
				{ "Taiwan North", 196L },
				{ "Taiwan Northwest", 196L },
				{ "UAE Central", 120L },
				{ "UAE North", 124L },
				{ "UK South", 20L },
				{ "UK West", 22L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 132L },
				{ "West Europe", 10L },
				{ "West India", 130L },
				{ "West US", 153L },
				{ "West US 2", 154L },
				{ "West US 3", 154L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Germany West Central",
			new Dictionary<string, long>
			{
				{ "Australia Central", 246L },
				{ "Australia Central 2", 246L },
				{ "Australia East", 240L },
				{ "Australia Southeast", 238L },
				{ "Austria East", 6L },
				{ "Brazil South", 192L },
				{ "Brazil Southeast", 192L },
				{ "Canada Central", 98L },
				{ "Canada East", 108L },
				{ "Central India", 118L },
				{ "Central US", 112L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 188L },
				{ "East US", 86L },
				{ "East US 2", 90L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 10L },
				{ "France South", 17L },
				{ "Germany North", 10L },
				{ "Germany West Central", 0L },
				{ "Indonesia Central", 154L },
				{ "Israel Central", 112L },
				{ "Italy North", 6L },
				{ "Japan East", 220L },
				{ "Japan West", 220L },
				{ "Jio India Central", 118L },
				{ "Jio India West", 118L },
				{ "Korea Central", 216L },
				{ "Korea South", 209L },
				{ "Malaysia South", 154L },
				{ "Mexico Central", 148L },
				{ "New Zealand North", 246L },
				{ "North Central US", 106L },
				{ "North Europe", 25L },
				{ "Norway East", 26L },
				{ "Norway West", 20L },
				{ "Poland Central", 8L },
				{ "Qatar Central", 112L },
				{ "South Africa North", 178L },
				{ "South Africa West", 156L },
				{ "South Central US", 116L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 154L },
				{ "South India", 130L },
				{ "Spain Central", 10L },
				{ "Sweden Central", 26L },
				{ "Sweden South", 26L },
				{ "Switzerland North", 6L },
				{ "Switzerland West", 10L },
				{ "Taiwan North", 188L },
				{ "Taiwan Northwest", 188L },
				{ "UAE Central", 112L },
				{ "UAE North", 114L },
				{ "UK South", 14L },
				{ "UK West", 18L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 126L },
				{ "West Europe", 8L },
				{ "West India", 122L },
				{ "West US", 148L },
				{ "West US 2", 148L },
				{ "West US 3", 148L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Indonesia Central",
			new Dictionary<string, long>
			{
				{ "Australia Central", 94L },
				{ "Australia Central 2", 92L },
				{ "Australia East", 88L },
				{ "Australia Southeast", 85L },
				{ "Austria East", 148L },
				{ "Brazil South", 328L },
				{ "Brazil Southeast", 328L },
				{ "Canada Central", 216L },
				{ "Canada East", 224L },
				{ "Central India", 52L },
				{ "Central US", 194L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 33L },
				{ "East US", 218L },
				{ "East US 2", 226L },
				{
					"East US SLV",
					long.MaxValue
				},
				{ "East US STG", 218L },
				{ "France Central", 150L },
				{ "France South", 138L },
				{ "Germany North", 162L },
				{ "Germany West Central", 154L },
				{ "Indonesia Central", 0L },
				{ "Israel Central", 76L },
				{ "Italy North", 148L },
				{ "Japan East", 68L },
				{ "Japan West", 74L },
				{ "Jio India Central", 52L },
				{ "Jio India West", 52L },
				{ "Korea Central", 62L },
				{ "Korea South", 56L },
				{ "Malaysia South", 100L },
				{ "Mexico Central", 160L },
				{ "New Zealand North", 94L },
				{ "North Central US", 202L },
				{ "North Europe", 172L },
				{ "Norway East", 180L },
				{ "Norway West", 174L },
				{ "Poland Central", 160L },
				{ "Qatar Central", 76L },
				{ "South Africa North", 298L },
				{ "South Africa West", 302L },
				{ "South Central US", 200L },
				{ "South Central US STG", 200L },
				{ "Southeast Asia", 100L },
				{ "South India", 34L },
				{ "Spain Central", 150L },
				{ "Sweden Central", 180L },
				{ "Sweden South", 180L },
				{ "Switzerland North", 148L },
				{ "Switzerland West", 144L },
				{ "Taiwan North", 33L },
				{ "Taiwan Northwest", 33L },
				{ "UAE Central", 76L },
				{ "UAE North", 76L },
				{ "UK South", 158L },
				{ "UK West", 161L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 180L },
				{ "West Europe", 160L },
				{ "West India", 50L },
				{ "West US", 168L },
				{ "West US 2", 160L },
				{ "West US 3", 160L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Israel Central",
			new Dictionary<string, long>
			{
				{ "Australia Central", 168L },
				{ "Australia Central 2", 168L },
				{ "Australia East", 164L },
				{ "Australia Southeast", 160L },
				{ "Austria East", 106L },
				{ "Brazil South", 296L },
				{ "Brazil Southeast", 296L },
				{ "Canada Central", 201L },
				{ "Canada East", 210L },
				{ "Central India", 30L },
				{ "Central US", 216L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 110L },
				{ "East US", 189L },
				{ "East US 2", 190L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 108L },
				{ "France South", 96L },
				{ "Germany North", 120L },
				{ "Germany West Central", 112L },
				{ "Indonesia Central", 76L },
				{ "Israel Central", 0L },
				{ "Italy North", 106L },
				{ "Japan East", 144L },
				{ "Japan West", 149L },
				{ "Jio India Central", 30L },
				{ "Jio India West", 30L },
				{ "Korea Central", 138L },
				{ "Korea South", 132L },
				{ "Malaysia South", 76L },
				{ "Mexico Central", 236L },
				{ "New Zealand North", 168L },
				{ "North Central US", 209L },
				{ "North Europe", 129L },
				{ "Norway East", 138L },
				{ "Norway West", 130L },
				{ "Poland Central", 118L },
				{ "Qatar Central", 4L },
				{ "South Africa North", 256L },
				{ "South Africa West", 260L },
				{ "South Central US", 218L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 76L },
				{ "South India", 46L },
				{ "Spain Central", 108L },
				{ "Sweden Central", 129L },
				{ "Sweden South", 129L },
				{ "Switzerland North", 106L },
				{ "Switzerland West", 102L },
				{ "Taiwan North", 110L },
				{ "Taiwan Northwest", 110L },
				{ "UAE Central", 100L },
				{ "UAE North", 4L },
				{ "UK South", 116L },
				{ "UK West", 118L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 230L },
				{ "West Europe", 118L },
				{ "West India", 28L },
				{ "West US", 244L },
				{ "West US 2", 236L },
				{ "West US 3", 236L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Italy North",
			new Dictionary<string, long>
			{
				{ "Australia Central", 240L },
				{ "Australia Central 2", 240L },
				{ "Australia East", 236L },
				{ "Australia Southeast", 232L },
				{ "Austria East", 100L },
				{ "Brazil South", 198L },
				{ "Brazil Southeast", 198L },
				{ "Canada Central", 104L },
				{ "Canada East", 114L },
				{ "Central India", 112L },
				{ "Central US", 112L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 182L },
				{ "East US", 92L },
				{ "East US 2", 94L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 14L },
				{ "France South", 10L },
				{ "Germany North", 16L },
				{ "Germany West Central", 6L },
				{ "Indonesia Central", 148L },
				{ "Israel Central", 106L },
				{ "Italy North", 0L },
				{ "Japan East", 216L },
				{ "Japan West", 216L },
				{ "Jio India Central", 112L },
				{ "Jio India West", 112L },
				{ "Korea Central", 210L },
				{ "Korea South", 204L },
				{ "Malaysia South", 148L },
				{ "Mexico Central", 154L },
				{ "New Zealand North", 240L },
				{ "North Central US", 110L },
				{ "North Europe", 31L },
				{ "Norway East", 32L },
				{ "Norway West", 26L },
				{ "Poland Central", 12L },
				{ "Qatar Central", 106L },
				{ "South Africa North", 172L },
				{ "South Africa West", 162L },
				{ "South Central US", 118L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 148L },
				{ "South India", 126L },
				{ "Spain Central", 14L },
				{ "Sweden Central", 32L },
				{ "Sweden South", 255L },
				{ "Switzerland North", 100L },
				{ "Switzerland West", 4L },
				{ "Taiwan North", 182L },
				{ "Taiwan Northwest", 182L },
				{ "UAE Central", 106L },
				{ "UAE North", 108L },
				{ "UK South", 20L },
				{ "UK West", 22L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 134L },
				{ "West Europe", 12L },
				{ "West India", 116L },
				{ "West US", 150L },
				{ "West US 2", 154L },
				{ "West US 3", 154L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Japan East",
			new Dictionary<string, long>
			{
				{ "Australia Central", 125L },
				{ "Australia Central 2", 124L },
				{ "Australia East", 118L },
				{ "Australia Southeast", 130L },
				{ "Austria East", 216L },
				{ "Brazil South", 262L },
				{ "Brazil Southeast", 262L },
				{ "Canada Central", 152L },
				{ "Canada East", 162L },
				{ "Central India", 120L },
				{ "Central US", 140L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 50L },
				{ "East US", 162L },
				{ "East US 2", 164L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 218L },
				{ "France South", 204L },
				{ "Germany North", 230L },
				{ "Germany West Central", 220L },
				{ "Indonesia Central", 68L },
				{ "Israel Central", 144L },
				{ "Italy North", 216L },
				{ "Japan East", 0L },
				{ "Japan West", 8L },
				{ "Jio India Central", 120L },
				{ "Jio India West", 120L },
				{ "Korea Central", 30L },
				{ "Korea South", 35L },
				{ "Malaysia South", 68L },
				{ "Mexico Central", 112L },
				{ "New Zealand North", 125L },
				{ "North Central US", 148L },
				{ "North Europe", 223L },
				{ "Norway East", 254L },
				{ "Norway West", 248L },
				{ "Poland Central", 234L },
				{ "Qatar Central", 144L },
				{ "South Africa North", 366L },
				{ "South Africa West", 370L },
				{ "South Central US", 138L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 68L },
				{ "South India", 100L },
				{ "Spain Central", 218L },
				{ "Sweden Central", 254L },
				{ "Sweden South", 254L },
				{ "Switzerland North", 216L },
				{ "Switzerland West", 212L },
				{ "Taiwan North", 50L },
				{ "Taiwan Northwest", 50L },
				{ "UAE Central", 144L },
				{ "UAE North", 144L },
				{ "UK South", 226L },
				{ "UK West", 230L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 126L },
				{ "West Europe", 234L },
				{ "West India", 118L },
				{ "West US", 106L },
				{ "West US 2", 112L },
				{ "West US 3", 112L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Japan West",
			new Dictionary<string, long>
			{
				{ "Australia Central", 130L },
				{ "Australia Central 2", 132L },
				{ "Australia East", 126L },
				{ "Australia Southeast", 138L },
				{ "Austria East", 216L },
				{ "Brazil South", 270L },
				{ "Brazil Southeast", 270L },
				{ "Canada Central", 160L },
				{ "Canada East", 170L },
				{ "Central India", 124L },
				{ "Central US", 143L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 50L },
				{ "East US", 168L },
				{ "East US 2", 164L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 218L },
				{ "France South", 206L },
				{ "Germany North", 230L },
				{ "Germany West Central", 220L },
				{ "Indonesia Central", 74L },
				{ "Israel Central", 149L },
				{ "Italy North", 216L },
				{ "Japan East", 8L },
				{ "Japan West", 0L },
				{ "Jio India Central", 124L },
				{ "Jio India West", 124L },
				{ "Korea Central", 36L },
				{ "Korea South", 42L },
				{ "Malaysia South", 74L },
				{ "Mexico Central", 112L },
				{ "New Zealand North", 130L },
				{ "North Central US", 152L },
				{ "North Europe", 236L },
				{ "Norway East", 248L },
				{ "Norway West", 240L },
				{ "Poland Central", 228L },
				{ "Qatar Central", 149L },
				{ "South Africa North", 366L },
				{ "South Africa West", 370L },
				{ "South Central US", 138L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 74L },
				{ "South India", 104L },
				{ "Spain Central", 218L },
				{ "Sweden Central", 248L },
				{ "Sweden South", 248L },
				{ "Switzerland North", 216L },
				{ "Switzerland West", 210L },
				{ "Taiwan North", 50L },
				{ "Taiwan Northwest", 50L },
				{ "UAE Central", 149L },
				{ "UAE North", 148L },
				{ "UK South", 225L },
				{ "UK West", 228L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 128L },
				{ "West Europe", 228L },
				{ "West India", 122L },
				{ "West US", 106L },
				{ "West US 2", 112L },
				{ "West US 3", 112L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Jio India Central",
			new Dictionary<string, long>
			{
				{ "Australia Central", 144L },
				{ "Australia Central 2", 144L },
				{ "Australia East", 140L },
				{ "Australia Southeast", 136L },
				{ "Austria East", 112L },
				{ "Brazil South", 302L },
				{ "Brazil Southeast", 302L },
				{ "Canada Central", 208L },
				{ "Canada East", 218L },
				{ "Central India", 100L },
				{ "Central US", 222L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 86L },
				{ "East US", 196L },
				{ "East US 2", 198L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 116L },
				{ "France South", 102L },
				{ "Germany North", 128L },
				{ "Germany West Central", 118L },
				{ "Indonesia Central", 52L },
				{ "Israel Central", 30L },
				{ "Italy North", 112L },
				{ "Japan East", 120L },
				{ "Japan West", 124L },
				{ "Jio India Central", 0L },
				{ "Jio India West", 100L },
				{ "Korea Central", 114L },
				{ "Korea South", 106L },
				{ "Malaysia South", 52L },
				{ "Mexico Central", 212L },
				{ "New Zealand North", 144L },
				{ "North Central US", 216L },
				{ "North Europe", 137L },
				{ "Norway East", 146L },
				{ "Norway West", 136L },
				{ "Poland Central", 126L },
				{ "Qatar Central", 30L },
				{ "South Africa North", 264L },
				{ "South Africa West", 267L },
				{ "South Central US", 224L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 52L },
				{ "South India", 22L },
				{ "Spain Central", 116L },
				{ "Sweden Central", 146L },
				{ "Sweden South", 146L },
				{ "Switzerland North", 112L },
				{ "Switzerland West", 110L },
				{ "Taiwan North", 86L },
				{ "Taiwan Northwest", 86L },
				{ "UAE Central", 30L },
				{ "UAE North", 30L },
				{ "UK South", 122L },
				{ "UK West", 126L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 236L },
				{ "West Europe", 126L },
				{ "West India", 4L },
				{ "West US", 218L },
				{ "West US 2", 212L },
				{ "West US 3", 212L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Jio India West",
			new Dictionary<string, long>
			{
				{ "Australia Central", 144L },
				{ "Australia Central 2", 144L },
				{ "Australia East", 140L },
				{ "Australia Southeast", 136L },
				{ "Austria East", 112L },
				{ "Brazil South", 302L },
				{ "Brazil Southeast", 302L },
				{ "Canada Central", 208L },
				{ "Canada East", 218L },
				{ "Central India", 100L },
				{ "Central US", 222L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 86L },
				{ "East US", 196L },
				{ "East US 2", 198L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 116L },
				{ "France South", 102L },
				{ "Germany North", 128L },
				{ "Germany West Central", 118L },
				{ "Indonesia Central", 52L },
				{ "Israel Central", 30L },
				{ "Italy North", 112L },
				{ "Japan East", 120L },
				{ "Japan West", 124L },
				{ "Jio India Central", 100L },
				{ "Jio India West", 0L },
				{ "Korea Central", 114L },
				{ "Korea South", 106L },
				{ "Malaysia South", 52L },
				{ "Mexico Central", 212L },
				{ "New Zealand North", 144L },
				{ "North Central US", 216L },
				{ "North Europe", 137L },
				{ "Norway East", 146L },
				{ "Norway West", 136L },
				{ "Poland Central", 126L },
				{ "Qatar Central", 30L },
				{ "South Africa North", 264L },
				{ "South Africa West", 267L },
				{ "South Central US", 224L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 52L },
				{ "South India", 22L },
				{ "Spain Central", 116L },
				{ "Sweden Central", 146L },
				{ "Sweden South", 146L },
				{ "Switzerland North", 112L },
				{ "Switzerland West", 110L },
				{ "Taiwan North", 86L },
				{ "Taiwan Northwest", 86L },
				{ "UAE Central", 30L },
				{ "UAE North", 30L },
				{ "UK South", 122L },
				{ "UK West", 126L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 236L },
				{ "West Europe", 126L },
				{ "West India", 4L },
				{ "West US", 218L },
				{ "West US 2", 212L },
				{ "West US 3", 212L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Korea Central",
			new Dictionary<string, long>
			{
				{ "Australia Central", 154L },
				{ "Australia Central 2", 154L },
				{ "Australia East", 150L },
				{ "Australia Southeast", 146L },
				{ "Austria East", 210L },
				{ "Brazil South", 300L },
				{ "Brazil Southeast", 300L },
				{ "Canada Central", 178L },
				{ "Canada East", 186L },
				{ "Central India", 114L },
				{ "Central US", 158L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 56L },
				{ "East US", 182L },
				{ "East US 2", 176L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 212L },
				{ "France South", 200L },
				{ "Germany North", 224L },
				{ "Germany West Central", 216L },
				{ "Indonesia Central", 62L },
				{ "Israel Central", 138L },
				{ "Italy North", 210L },
				{ "Japan East", 30L },
				{ "Japan West", 36L },
				{ "Jio India Central", 114L },
				{ "Jio India West", 114L },
				{ "Korea Central", 0L },
				{ "Korea South", 6L },
				{ "Malaysia South", 62L },
				{ "Mexico Central", 122L },
				{ "New Zealand North", 154L },
				{ "North Central US", 165L },
				{ "North Europe", 233L },
				{ "Norway East", 242L },
				{ "Norway West", 236L },
				{ "Poland Central", 222L },
				{ "Qatar Central", 138L },
				{ "South Africa North", 360L },
				{ "South Africa West", 364L },
				{ "South Central US", 152L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 62L },
				{ "South India", 95L },
				{ "Spain Central", 212L },
				{ "Sweden Central", 242L },
				{ "Sweden South", 242L },
				{ "Switzerland North", 210L },
				{ "Switzerland West", 206L },
				{ "Taiwan North", 56L },
				{ "Taiwan Northwest", 56L },
				{ "UAE Central", 138L },
				{ "UAE North", 138L },
				{ "UK South", 220L },
				{ "UK West", 222L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 143L },
				{ "West Europe", 222L },
				{ "West India", 112L },
				{ "West US", 130L },
				{ "West US 2", 122L },
				{ "West US 3", 122L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Korea South",
			new Dictionary<string, long>
			{
				{ "Australia Central", 148L },
				{ "Australia Central 2", 148L },
				{ "Australia East", 143L },
				{ "Australia Southeast", 140L },
				{ "Austria East", 204L },
				{ "Brazil South", 306L },
				{ "Brazil Southeast", 306L },
				{ "Canada Central", 182L },
				{ "Canada East", 192L },
				{ "Central India", 106L },
				{ "Central US", 162L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 60L },
				{ "East US", 186L },
				{ "East US 2", 182L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 206L },
				{ "France South", 193L },
				{ "Germany North", 218L },
				{ "Germany West Central", 209L },
				{ "Indonesia Central", 56L },
				{ "Israel Central", 132L },
				{ "Italy North", 204L },
				{ "Japan East", 35L },
				{ "Japan West", 42L },
				{ "Jio India Central", 106L },
				{ "Jio India West", 106L },
				{ "Korea Central", 6L },
				{ "Korea South", 0L },
				{ "Malaysia South", 56L },
				{ "Mexico Central", 126L },
				{ "New Zealand North", 148L },
				{ "North Central US", 170L },
				{ "North Europe", 226L },
				{ "Norway East", 235L },
				{ "Norway West", 228L },
				{ "Poland Central", 216L },
				{ "Qatar Central", 132L },
				{ "South Africa North", 354L },
				{ "South Africa West", 358L },
				{ "South Central US", 156L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 56L },
				{ "South India", 88L },
				{ "Spain Central", 206L },
				{ "Sweden Central", 235L },
				{ "Sweden South", 235L },
				{ "Switzerland North", 204L },
				{ "Switzerland West", 200L },
				{ "Taiwan North", 60L },
				{ "Taiwan Northwest", 60L },
				{ "UAE Central", 132L },
				{ "UAE North", 130L },
				{ "UK South", 214L },
				{ "UK West", 216L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 148L },
				{ "West Europe", 216L },
				{ "West India", 106L },
				{ "West US", 134L },
				{ "West US 2", 126L },
				{ "West US 3", 126L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Malaysia South",
			new Dictionary<string, long>
			{
				{ "Australia Central", 94L },
				{ "Australia Central 2", 92L },
				{ "Australia East", 88L },
				{ "Australia Southeast", 85L },
				{ "Austria East", 148L },
				{ "Brazil South", 328L },
				{ "Brazil Southeast", 328L },
				{ "Canada Central", 216L },
				{ "Canada East", 224L },
				{ "Central India", 52L },
				{ "Central US", 194L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 33L },
				{ "East US", 218L },
				{ "East US 2", 226L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 150L },
				{ "France South", 138L },
				{ "Germany North", 162L },
				{ "Germany West Central", 154L },
				{ "Indonesia Central", 100L },
				{ "Israel Central", 76L },
				{ "Italy North", 148L },
				{ "Japan East", 68L },
				{ "Japan West", 74L },
				{ "Jio India Central", 52L },
				{ "Jio India West", 52L },
				{ "Korea Central", 62L },
				{ "Korea South", 56L },
				{ "Malaysia South", 0L },
				{ "Mexico Central", 160L },
				{ "New Zealand North", 94L },
				{ "North Central US", 202L },
				{ "North Europe", 172L },
				{ "Norway East", 180L },
				{ "Norway West", 174L },
				{ "Poland Central", 160L },
				{ "Qatar Central", 76L },
				{ "South Africa North", 298L },
				{ "South Africa West", 302L },
				{ "South Central US", 200L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 100L },
				{ "South India", 34L },
				{ "Spain Central", 150L },
				{ "Sweden Central", 180L },
				{ "Sweden South", 180L },
				{ "Switzerland North", 148L },
				{ "Switzerland West", 144L },
				{ "Taiwan North", 33L },
				{ "Taiwan Northwest", 33L },
				{ "UAE Central", 76L },
				{ "UAE North", 76L },
				{ "UK South", 158L },
				{ "UK West", 161L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 180L },
				{ "West Europe", 160L },
				{ "West India", 50L },
				{ "West US", 168L },
				{ "West US 2", 160L },
				{ "West US 3", 160L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Mexico Central",
			new Dictionary<string, long>
			{
				{ "Australia Central", 162L },
				{ "Australia Central 2", 162L },
				{ "Australia East", 158L },
				{ "Australia Southeast", 168L },
				{ "Austria East", 154L },
				{ "Brazil South", 182L },
				{ "Brazil Southeast", 182L },
				{ "Canada Central", 57L },
				{ "Canada East", 66L },
				{ "Central India", 212L },
				{ "Central US", 36L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 140L },
				{ "East US", 58L },
				{ "East US 2", 68L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 140L },
				{ "France South", 146L },
				{ "Germany North", 154L },
				{ "Germany West Central", 148L },
				{ "Indonesia Central", 160L },
				{ "Israel Central", 236L },
				{ "Italy North", 154L },
				{ "Japan East", 112L },
				{ "Japan West", 112L },
				{ "Jio India Central", 212L },
				{ "Jio India West", 212L },
				{ "Korea Central", 122L },
				{ "Korea South", 126L },
				{ "Malaysia South", 160L },
				{ "Mexico Central", 0L },
				{ "New Zealand North", 162L },
				{ "North Central US", 44L },
				{ "North Europe", 136L },
				{ "Norway East", 164L },
				{ "Norway West", 156L },
				{ "Poland Central", 142L },
				{ "Qatar Central", 236L },
				{ "South Africa North", 296L },
				{ "South Africa West", 280L },
				{ "South Central US", 44L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 160L },
				{ "South India", 192L },
				{ "Spain Central", 140L },
				{ "Sweden Central", 164L },
				{ "Sweden South", 164L },
				{ "Switzerland North", 154L },
				{ "Switzerland West", 146L },
				{ "Taiwan North", 140L },
				{ "Taiwan Northwest", 140L },
				{ "UAE Central", 236L },
				{ "UAE North", 236L },
				{ "UK South", 130L },
				{ "UK West", 132L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 22L },
				{ "West Europe", 142L },
				{ "West India", 210L },
				{ "West US", 22L },
				{ "West US 2", 100L },
				{ "West US 3", 100L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"New Zealand North",
			new Dictionary<string, long>
			{
				{ "Australia Central", 100L },
				{ "Australia Central 2", 2L },
				{ "Australia East", 6L },
				{ "Australia Southeast", 18L },
				{ "Austria East", 240L },
				{ "Brazil South", 314L },
				{ "Brazil Southeast", 314L },
				{ "Canada Central", 204L },
				{ "Canada East", 214L },
				{ "Central India", 144L },
				{ "Central US", 184L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 120L },
				{ "East US", 205L },
				{ "East US 2", 200L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 242L },
				{ "France South", 230L },
				{ "Germany North", 254L },
				{ "Germany West Central", 246L },
				{ "Indonesia Central", 94L },
				{ "Israel Central", 168L },
				{ "Italy North", 240L },
				{ "Japan East", 125L },
				{ "Japan West", 130L },
				{ "Jio India Central", 144L },
				{ "Jio India West", 144L },
				{ "Korea Central", 154L },
				{ "Korea South", 148L },
				{ "Malaysia South", 94L },
				{ "Mexico Central", 162L },
				{ "New Zealand North", 0L },
				{ "North Central US", 192L },
				{ "North Europe", 262L },
				{ "Norway East", 272L },
				{ "Norway West", 266L },
				{ "Poland Central", 252L },
				{ "Qatar Central", 168L },
				{ "South Africa North", 390L },
				{ "South Africa West", 394L },
				{ "South Central US", 174L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 94L },
				{ "South India", 126L },
				{ "Spain Central", 242L },
				{ "Sweden Central", 272L },
				{ "Sweden South", 272L },
				{ "Switzerland North", 240L },
				{ "Switzerland West", 236L },
				{ "Taiwan North", 120L },
				{ "Taiwan Northwest", 120L },
				{ "UAE Central", 168L },
				{ "UAE North", 168L },
				{ "UK South", 250L },
				{ "UK West", 252L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 170L },
				{ "West Europe", 252L },
				{ "West India", 142L },
				{ "West US", 142L },
				{ "West US 2", 162L },
				{ "West US 3", 162L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"North Central US",
			new Dictionary<string, long>
			{
				{ "Australia Central", 192L },
				{ "Australia Central 2", 192L },
				{ "Australia East", 187L },
				{ "Australia Southeast", 198L },
				{ "Austria East", 110L },
				{ "Brazil South", 138L },
				{ "Brazil Southeast", 138L },
				{ "Canada Central", 14L },
				{ "Canada East", 24L },
				{ "Central India", 216L },
				{ "Central US", 8L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 184L },
				{ "East US", 19L },
				{ "East US 2", 22L },
				{
					"East US SLV",
					long.MaxValue
				},
				{ "East US STG", 19L },
				{ "France Central", 96L },
				{ "France South", 104L },
				{ "Germany North", 112L },
				{ "Germany West Central", 106L },
				{ "Indonesia Central", 202L },
				{ "Israel Central", 209L },
				{ "Italy North", 110L },
				{ "Japan East", 148L },
				{ "Japan West", 152L },
				{ "Jio India Central", 216L },
				{ "Jio India West", 216L },
				{ "Korea Central", 165L },
				{ "Korea South", 170L },
				{ "Malaysia South", 202L },
				{ "Mexico Central", 44L },
				{ "New Zealand North", 192L },
				{ "North Central US", 0L },
				{ "North Europe", 90L },
				{ "Norway East", 122L },
				{ "Norway West", 114L },
				{ "Poland Central", 102L },
				{ "Qatar Central", 209L },
				{ "South Africa North", 253L },
				{ "South Africa West", 236L },
				{ "South Central US", 30L },
				{ "South Central US STG", 30L },
				{ "Southeast Asia", 202L },
				{ "South India", 228L },
				{ "Spain Central", 96L },
				{ "Sweden Central", 122L },
				{ "Sweden South", 122L },
				{ "Switzerland North", 110L },
				{ "Switzerland West", 106L },
				{ "Taiwan North", 184L },
				{ "Taiwan Northwest", 184L },
				{ "UAE Central", 209L },
				{ "UAE North", 212L },
				{ "UK South", 90L },
				{ "UK West", 92L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 22L },
				{ "West Europe", 102L },
				{ "West India", 218L },
				{ "West US", 50L },
				{ "West US 2", 44L },
				{ "West US 3", 44L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"North Europe",
			new Dictionary<string, long>
			{
				{ "Australia Central", 262L },
				{ "Australia Central 2", 262L },
				{ "Australia East", 258L },
				{ "Australia Southeast", 260L },
				{ "Austria East", 31L },
				{ "Brazil South", 177L },
				{ "Brazil Southeast", 177L },
				{ "Canada Central", 84L },
				{ "Canada East", 93L },
				{ "Central India", 137L },
				{ "Central US", 92L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 205L },
				{ "East US", 76L },
				{ "East US 2", 75L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 27L },
				{ "France South", 31L },
				{ "Germany North", 31L },
				{ "Germany West Central", 25L },
				{ "Indonesia Central", 172L },
				{ "Israel Central", 129L },
				{ "Italy North", 31L },
				{ "Japan East", 223L },
				{ "Japan West", 236L },
				{ "Jio India Central", 137L },
				{ "Jio India West", 137L },
				{ "Korea Central", 233L },
				{ "Korea South", 226L },
				{ "Malaysia South", 172L },
				{ "Mexico Central", 136L },
				{ "New Zealand North", 262L },
				{ "North Central US", 90L },
				{ "North Europe", 0L },
				{ "Norway East", 40L },
				{ "Norway West", 33L },
				{ "Poland Central", 22L },
				{ "Qatar Central", 129L },
				{ "South Africa North", 179L },
				{ "South Africa West", 157L },
				{ "South Central US", 106L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 172L },
				{ "South India", 149L },
				{ "Spain Central", 27L },
				{ "Sweden Central", 40L },
				{ "Sweden South", 40L },
				{ "Switzerland North", 31L },
				{ "Switzerland West", 30L },
				{ "Taiwan North", 205L },
				{ "Taiwan Northwest", 205L },
				{ "UAE Central", 129L },
				{ "UAE North", 132L },
				{ "UK South", 21L },
				{ "UK West", 17L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 110L },
				{ "West Europe", 22L },
				{ "West India", 139L },
				{ "West US", 133L },
				{ "West US 2", 136L },
				{ "West US 3", 136L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Norway East",
			new Dictionary<string, long>
			{
				{ "Australia Central", 272L },
				{ "Australia Central 2", 272L },
				{ "Australia East", 266L },
				{ "Australia Southeast", 264L },
				{ "Austria East", 32L },
				{ "Brazil South", 208L },
				{ "Brazil Southeast", 208L },
				{ "Canada Central", 114L },
				{ "Canada East", 124L },
				{ "Central India", 146L },
				{ "Central US", 128L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 214L },
				{ "East US", 101L },
				{ "East US 2", 104L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 30L },
				{ "France South", 42L },
				{ "Germany North", 20L },
				{ "Germany West Central", 26L },
				{ "Indonesia Central", 180L },
				{ "Israel Central", 138L },
				{ "Italy North", 32L },
				{ "Japan East", 254L },
				{ "Japan West", 248L },
				{ "Jio India Central", 146L },
				{ "Jio India West", 146L },
				{ "Korea Central", 242L },
				{ "Korea South", 235L },
				{ "Malaysia South", 180L },
				{ "Mexico Central", 164L },
				{ "New Zealand North", 272L },
				{ "North Central US", 122L },
				{ "North Europe", 40L },
				{ "Norway East", 0L },
				{ "Norway West", 8L },
				{ "Poland Central", 22L },
				{ "Qatar Central", 138L },
				{ "South Africa North", 186L },
				{ "South Africa West", 170L },
				{ "South Central US", 132L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 180L },
				{ "South India", 157L },
				{ "Spain Central", 30L },
				{ "Sweden Central", 20L },
				{ "Sweden South", 20L },
				{ "Switzerland North", 32L },
				{ "Switzerland West", 36L },
				{ "Taiwan North", 214L },
				{ "Taiwan Northwest", 214L },
				{ "UAE Central", 138L },
				{ "UAE North", 140L },
				{ "UK South", 28L },
				{ "UK West", 30L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 142L },
				{ "West Europe", 22L },
				{ "West India", 146L },
				{ "West US", 163L },
				{ "West US 2", 164L },
				{ "West US 3", 164L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Norway West",
			new Dictionary<string, long>
			{
				{ "Australia Central", 266L },
				{ "Australia Central 2", 266L },
				{ "Australia East", 260L },
				{ "Australia Southeast", 258L },
				{ "Austria East", 26L },
				{ "Brazil South", 201L },
				{ "Brazil Southeast", 201L },
				{ "Canada Central", 108L },
				{ "Canada East", 116L },
				{ "Central India", 136L },
				{ "Central US", 121L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 206L },
				{ "East US", 94L },
				{ "East US 2", 98L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 24L },
				{ "France South", 36L },
				{ "Germany North", 26L },
				{ "Germany West Central", 20L },
				{ "Indonesia Central", 174L },
				{ "Israel Central", 130L },
				{ "Italy North", 26L },
				{ "Japan East", 248L },
				{ "Japan West", 240L },
				{ "Jio India Central", 136L },
				{ "Jio India West", 136L },
				{ "Korea Central", 236L },
				{ "Korea South", 228L },
				{ "Malaysia South", 174L },
				{ "Mexico Central", 156L },
				{ "New Zealand North", 266L },
				{ "North Central US", 114L },
				{ "North Europe", 33L },
				{ "Norway East", 8L },
				{ "Norway West", 0L },
				{ "Poland Central", 15L },
				{ "Qatar Central", 130L },
				{ "South Africa North", 179L },
				{ "South Africa West", 162L },
				{ "South Central US", 124L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 174L },
				{ "South India", 150L },
				{ "Spain Central", 24L },
				{ "Sweden Central", 20L },
				{ "Sweden South", 20L },
				{ "Switzerland North", 26L },
				{ "Switzerland West", 30L },
				{ "Taiwan North", 206L },
				{ "Taiwan Northwest", 206L },
				{ "UAE Central", 130L },
				{ "UAE North", 134L },
				{ "UK South", 22L },
				{ "UK West", 24L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 136L },
				{ "West Europe", 15L },
				{ "West India", 140L },
				{ "West US", 156L },
				{ "West US 2", 156L },
				{ "West US 3", 156L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Poland Central",
			new Dictionary<string, long>
			{
				{ "Australia Central", 252L },
				{ "Australia Central 2", 252L },
				{ "Australia East", 248L },
				{ "Australia Southeast", 244L },
				{ "Austria East", 12L },
				{ "Brazil South", 188L },
				{ "Brazil Southeast", 188L },
				{ "Canada Central", 94L },
				{ "Canada East", 103L },
				{ "Central India", 126L },
				{ "Central US", 102L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 193L },
				{ "East US", 81L },
				{ "East US 2", 86L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 10L },
				{ "France South", 20L },
				{ "Germany North", 10L },
				{ "Germany West Central", 8L },
				{ "Indonesia Central", 160L },
				{ "Israel Central", 118L },
				{ "Italy North", 12L },
				{ "Japan East", 234L },
				{ "Japan West", 228L },
				{ "Jio India Central", 126L },
				{ "Jio India West", 126L },
				{ "Korea Central", 222L },
				{ "Korea South", 216L },
				{ "Malaysia South", 160L },
				{ "Mexico Central", 142L },
				{ "New Zealand North", 252L },
				{ "North Central US", 102L },
				{ "North Europe", 22L },
				{ "Norway East", 22L },
				{ "Norway West", 15L },
				{ "Poland Central", 0L },
				{ "Qatar Central", 118L },
				{ "South Africa North", 170L },
				{ "South Africa West", 152L },
				{ "South Central US", 112L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 160L },
				{ "South India", 136L },
				{ "Spain Central", 10L },
				{ "Sweden Central", 22L },
				{ "Sweden South", 22L },
				{ "Switzerland North", 12L },
				{ "Switzerland West", 16L },
				{ "Taiwan North", 193L },
				{ "Taiwan Northwest", 193L },
				{ "UAE Central", 118L },
				{ "UAE North", 120L },
				{ "UK South", 10L },
				{ "UK West", 12L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 120L },
				{ "West Europe", 100L },
				{ "West India", 128L },
				{ "West US", 144L },
				{ "West US 2", 142L },
				{ "West US 3", 142L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Qatar Central",
			new Dictionary<string, long>
			{
				{ "Australia Central", 168L },
				{ "Australia Central 2", 168L },
				{ "Australia East", 164L },
				{ "Australia Southeast", 160L },
				{ "Austria East", 106L },
				{ "Brazil South", 296L },
				{ "Brazil Southeast", 296L },
				{ "Canada Central", 201L },
				{ "Canada East", 210L },
				{ "Central India", 30L },
				{ "Central US", 216L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 110L },
				{ "East US", 189L },
				{ "East US 2", 190L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 108L },
				{ "France South", 96L },
				{ "Germany North", 120L },
				{ "Germany West Central", 112L },
				{ "Indonesia Central", 76L },
				{ "Israel Central", 4L },
				{ "Italy North", 106L },
				{ "Japan East", 144L },
				{ "Japan West", 149L },
				{ "Jio India Central", 30L },
				{ "Jio India West", 30L },
				{ "Korea Central", 138L },
				{ "Korea South", 132L },
				{ "Malaysia South", 76L },
				{ "Mexico Central", 236L },
				{ "New Zealand North", 168L },
				{ "North Central US", 209L },
				{ "North Europe", 129L },
				{ "Norway East", 138L },
				{ "Norway West", 130L },
				{ "Poland Central", 118L },
				{ "Qatar Central", 0L },
				{ "South Africa North", 256L },
				{ "South Africa West", 260L },
				{ "South Central US", 218L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 76L },
				{ "South India", 46L },
				{ "Spain Central", 108L },
				{ "Sweden Central", 129L },
				{ "Sweden South", 129L },
				{ "Switzerland North", 106L },
				{ "Switzerland West", 102L },
				{ "Taiwan North", 110L },
				{ "Taiwan Northwest", 110L },
				{ "UAE Central", 4L },
				{ "UAE North", 4L },
				{ "UK South", 116L },
				{ "UK West", 118L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 230L },
				{ "West Europe", 118L },
				{ "West India", 28L },
				{ "West US", 244L },
				{ "West US 2", 236L },
				{ "West US 3", 236L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"South Africa North",
			new Dictionary<string, long>
			{
				{ "Australia Central", 390L },
				{ "Australia Central 2", 391L },
				{ "Australia East", 386L },
				{ "Australia Southeast", 382L },
				{ "Austria East", 172L },
				{ "Brazil South", 342L },
				{ "Brazil Southeast", 342L },
				{ "Canada Central", 246L },
				{ "Canada East", 256L },
				{ "Central India", 264L },
				{ "Central US", 260L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 332L },
				{ "East US", 234L },
				{ "East US 2", 240L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 166L },
				{ "France South", 162L },
				{ "Germany North", 178L },
				{ "Germany West Central", 178L },
				{ "Indonesia Central", 298L },
				{ "Israel Central", 256L },
				{ "Italy North", 172L },
				{ "Japan East", 366L },
				{ "Japan West", 366L },
				{ "Jio India Central", 264L },
				{ "Jio India West", 264L },
				{ "Korea Central", 360L },
				{ "Korea South", 354L },
				{ "Malaysia South", 298L },
				{ "Mexico Central", 296L },
				{ "New Zealand North", 390L },
				{ "North Central US", 253L },
				{ "North Europe", 179L },
				{ "Norway East", 186L },
				{ "Norway West", 179L },
				{ "Poland Central", 170L },
				{ "Qatar Central", 256L },
				{ "South Africa North", 0L },
				{ "South Africa West", 18L },
				{ "South Central US", 264L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 298L },
				{ "South India", 276L },
				{ "Spain Central", 166L },
				{ "Sweden Central", 186L },
				{ "Sweden South", 186L },
				{ "Switzerland North", 172L },
				{ "Switzerland West", 168L },
				{ "Taiwan North", 332L },
				{ "Taiwan Northwest", 332L },
				{ "UAE Central", 256L },
				{ "UAE North", 260L },
				{ "UK South", 160L },
				{ "UK West", 164L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 274L },
				{ "West Europe", 170L },
				{ "West India", 266L },
				{ "West US", 298L },
				{ "West US 2", 296L },
				{ "West US 3", 296L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"South Africa West",
			new Dictionary<string, long>
			{
				{ "Australia Central", 394L },
				{ "Australia Central 2", 392L },
				{ "Australia East", 390L },
				{ "Australia Southeast", 386L },
				{ "Austria East", 162L },
				{ "Brazil South", 326L },
				{ "Brazil Southeast", 326L },
				{ "Canada Central", 230L },
				{ "Canada East", 238L },
				{ "Central India", 267L },
				{ "Central US", 244L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 336L },
				{ "East US", 218L },
				{ "East US 2", 222L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 150L },
				{ "France South", 166L },
				{ "Germany North", 162L },
				{ "Germany West Central", 156L },
				{ "Indonesia Central", 302L },
				{ "Israel Central", 260L },
				{ "Italy North", 162L },
				{ "Japan East", 370L },
				{ "Japan West", 370L },
				{ "Jio India Central", 267L },
				{ "Jio India West", 267L },
				{ "Korea Central", 364L },
				{ "Korea South", 358L },
				{ "Malaysia South", 302L },
				{ "Mexico Central", 280L },
				{ "New Zealand North", 394L },
				{ "North Central US", 236L },
				{ "North Europe", 157L },
				{ "Norway East", 170L },
				{ "Norway West", 162L },
				{ "Poland Central", 152L },
				{ "Qatar Central", 260L },
				{ "South Africa North", 18L },
				{ "South Africa West", 0L },
				{ "South Central US", 248L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 302L },
				{ "South India", 280L },
				{ "Spain Central", 150L },
				{ "Sweden Central", 170L },
				{ "Sweden South", 170L },
				{ "Switzerland North", 162L },
				{ "Switzerland West", 158L },
				{ "Taiwan North", 336L },
				{ "Taiwan Northwest", 336L },
				{ "UAE Central", 260L },
				{ "UAE North", 264L },
				{ "UK South", 144L },
				{ "UK West", 146L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 258L },
				{ "West Europe", 152L },
				{ "West India", 270L },
				{ "West US", 280L },
				{ "West US 2", 280L },
				{ "West US 3", 280L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"South Central US",
			new Dictionary<string, long>
			{
				{ "Australia Central", 174L },
				{ "Australia Central 2", 174L },
				{ "Australia East", 170L },
				{ "Australia Southeast", 180L },
				{ "Austria East", 118L },
				{ "Brazil South", 140L },
				{ "Brazil Southeast", 140L },
				{ "Canada Central", 42L },
				{ "Canada East", 52L },
				{ "Central India", 224L },
				{ "Central US", 22L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 182L },
				{ "East US", 32L },
				{ "East US 2", 26L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 106L },
				{ "France South", 116L },
				{ "Germany North", 120L },
				{ "Germany West Central", 116L },
				{ "Indonesia Central", 200L },
				{ "Israel Central", 218L },
				{ "Italy North", 118L },
				{ "Japan East", 138L },
				{ "Japan West", 138L },
				{ "Jio India Central", 224L },
				{ "Jio India West", 224L },
				{ "Korea Central", 152L },
				{ "Korea South", 156L },
				{ "Malaysia South", 200L },
				{ "Mexico Central", 44L },
				{ "New Zealand North", 174L },
				{ "North Central US", 30L },
				{ "North Europe", 106L },
				{ "Norway East", 132L },
				{ "Norway West", 124L },
				{ "Poland Central", 112L },
				{ "Qatar Central", 218L },
				{ "South Africa North", 264L },
				{ "South Africa West", 248L },
				{ "South Central US", 0L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 200L },
				{ "South India", 222L },
				{ "Spain Central", 106L },
				{ "Sweden Central", 132L },
				{ "Sweden South", 132L },
				{ "Switzerland North", 118L },
				{ "Switzerland West", 116L },
				{ "Taiwan North", 182L },
				{ "Taiwan Northwest", 182L },
				{ "UAE Central", 218L },
				{ "UAE North", 220L },
				{ "UK South", 104L },
				{ "UK West", 106L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 22L },
				{ "West Europe", 112L },
				{ "West India", 226L },
				{ "West US", 34L },
				{ "West US 2", 44L },
				{ "West US 3", 44L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"South Central US STG",
			new Dictionary<string, long>
			{
				{
					"Australia Central",
					long.MaxValue
				},
				{
					"Australia Central 2",
					long.MaxValue
				},
				{
					"Australia East",
					long.MaxValue
				},
				{
					"Australia Southeast",
					long.MaxValue
				},
				{
					"Austria East",
					long.MaxValue
				},
				{
					"Brazil South",
					long.MaxValue
				},
				{
					"Brazil Southeast",
					long.MaxValue
				},
				{
					"Canada Central",
					long.MaxValue
				},
				{
					"Canada East",
					long.MaxValue
				},
				{
					"Central India",
					long.MaxValue
				},
				{
					"Central US",
					long.MaxValue
				},
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{
					"East Asia",
					long.MaxValue
				},
				{
					"East US",
					long.MaxValue
				},
				{ "East US 2", 26L },
				{
					"East US SLV",
					long.MaxValue
				},
				{ "East US STG", 32L },
				{
					"France Central",
					long.MaxValue
				},
				{
					"France South",
					long.MaxValue
				},
				{
					"Germany North",
					long.MaxValue
				},
				{
					"Germany West Central",
					long.MaxValue
				},
				{ "Indonesia Central", 200L },
				{
					"Israel Central",
					long.MaxValue
				},
				{
					"Italy North",
					long.MaxValue
				},
				{
					"Japan East",
					long.MaxValue
				},
				{
					"Japan West",
					long.MaxValue
				},
				{
					"Jio India Central",
					long.MaxValue
				},
				{
					"Jio India West",
					long.MaxValue
				},
				{
					"Korea Central",
					long.MaxValue
				},
				{
					"Korea South",
					long.MaxValue
				},
				{
					"Malaysia South",
					long.MaxValue
				},
				{
					"Mexico Central",
					long.MaxValue
				},
				{
					"New Zealand North",
					long.MaxValue
				},
				{ "North Central US", 30L },
				{
					"North Europe",
					long.MaxValue
				},
				{
					"Norway East",
					long.MaxValue
				},
				{
					"Norway West",
					long.MaxValue
				},
				{
					"Poland Central",
					long.MaxValue
				},
				{
					"Qatar Central",
					long.MaxValue
				},
				{
					"South Africa North",
					long.MaxValue
				},
				{
					"South Africa West",
					long.MaxValue
				},
				{
					"South Central US",
					long.MaxValue
				},
				{ "South Central US STG", 0L },
				{ "Southeast Asia", 200L },
				{
					"South India",
					long.MaxValue
				},
				{
					"Spain Central",
					long.MaxValue
				},
				{
					"Sweden Central",
					long.MaxValue
				},
				{
					"Sweden South",
					long.MaxValue
				},
				{
					"Switzerland North",
					long.MaxValue
				},
				{
					"Switzerland West",
					long.MaxValue
				},
				{
					"Taiwan North",
					long.MaxValue
				},
				{
					"Taiwan Northwest",
					long.MaxValue
				},
				{
					"UAE Central",
					long.MaxValue
				},
				{
					"UAE North",
					long.MaxValue
				},
				{
					"UK South",
					long.MaxValue
				},
				{
					"UK West",
					long.MaxValue
				},
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{
					"West Central US",
					long.MaxValue
				},
				{
					"West Europe",
					long.MaxValue
				},
				{
					"West India",
					long.MaxValue
				},
				{
					"West US",
					long.MaxValue
				},
				{ "West US 2", 44L },
				{
					"West US 3",
					long.MaxValue
				},
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Southeast Asia",
			new Dictionary<string, long>
			{
				{ "Australia Central", 94L },
				{ "Australia Central 2", 92L },
				{ "Australia East", 88L },
				{ "Australia Southeast", 85L },
				{ "Austria East", 148L },
				{ "Brazil South", 328L },
				{ "Brazil Southeast", 328L },
				{ "Canada Central", 216L },
				{ "Canada East", 224L },
				{ "Central India", 52L },
				{ "Central US", 194L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 33L },
				{ "East US", 218L },
				{ "East US 2", 226L },
				{
					"East US SLV",
					long.MaxValue
				},
				{ "East US STG", 218L },
				{ "France Central", 150L },
				{ "France South", 138L },
				{ "Germany North", 162L },
				{ "Germany West Central", 154L },
				{ "Indonesia Central", 100L },
				{ "Israel Central", 76L },
				{ "Italy North", 148L },
				{ "Japan East", 68L },
				{ "Japan West", 74L },
				{ "Jio India Central", 52L },
				{ "Jio India West", 52L },
				{ "Korea Central", 62L },
				{ "Korea South", 56L },
				{ "Malaysia South", 100L },
				{ "Mexico Central", 160L },
				{ "New Zealand North", 94L },
				{ "North Central US", 202L },
				{ "North Europe", 172L },
				{ "Norway East", 180L },
				{ "Norway West", 174L },
				{ "Poland Central", 160L },
				{ "Qatar Central", 76L },
				{ "South Africa North", 298L },
				{ "South Africa West", 302L },
				{ "South Central US", 200L },
				{ "South Central US STG", 200L },
				{ "Southeast Asia", 0L },
				{ "South India", 34L },
				{ "Spain Central", 150L },
				{ "Sweden Central", 180L },
				{ "Sweden South", 180L },
				{ "Switzerland North", 148L },
				{ "Switzerland West", 144L },
				{ "Taiwan North", 33L },
				{ "Taiwan Northwest", 33L },
				{ "UAE Central", 76L },
				{ "UAE North", 76L },
				{ "UK South", 158L },
				{ "UK West", 161L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 180L },
				{ "West Europe", 160L },
				{ "West India", 50L },
				{ "West US", 168L },
				{ "West US 2", 160L },
				{ "West US 3", 160L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"South India",
			new Dictionary<string, long>
			{
				{ "Australia Central", 126L },
				{ "Australia Central 2", 124L },
				{ "Australia East", 120L },
				{ "Australia Southeast", 118L },
				{ "Austria East", 126L },
				{ "Brazil South", 314L },
				{ "Brazil Southeast", 314L },
				{ "Canada Central", 220L },
				{ "Canada East", 230L },
				{ "Central India", 22L },
				{ "Central US", 228L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 66L },
				{ "East US", 208L },
				{ "East US 2", 210L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 128L },
				{ "France South", 114L },
				{ "Germany North", 140L },
				{ "Germany West Central", 130L },
				{ "Indonesia Central", 34L },
				{ "Israel Central", 46L },
				{ "Italy North", 126L },
				{ "Japan East", 100L },
				{ "Japan West", 104L },
				{ "Jio India Central", 22L },
				{ "Jio India West", 22L },
				{ "Korea Central", 95L },
				{ "Korea South", 88L },
				{ "Malaysia South", 34L },
				{ "Mexico Central", 192L },
				{ "New Zealand North", 126L },
				{ "North Central US", 228L },
				{ "North Europe", 149L },
				{ "Norway East", 157L },
				{ "Norway West", 150L },
				{ "Poland Central", 136L },
				{ "Qatar Central", 46L },
				{ "South Africa North", 276L },
				{ "South Africa West", 280L },
				{ "South Central US", 222L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 34L },
				{ "South India", 0L },
				{ "Spain Central", 128L },
				{ "Sweden Central", 157L },
				{ "Sweden South", 157L },
				{ "Switzerland North", 126L },
				{ "Switzerland West", 122L },
				{ "Taiwan North", 66L },
				{ "Taiwan Northwest", 66L },
				{ "UAE Central", 46L },
				{ "UAE North", 46L },
				{ "UK South", 134L },
				{ "UK West", 138L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 212L },
				{ "West Europe", 136L },
				{ "West India", 20L },
				{ "West US", 200L },
				{ "West US 2", 192L },
				{ "West US 3", 192L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Spain Central",
			new Dictionary<string, long>
			{
				{ "Australia Central", 242L },
				{ "Australia Central 2", 242L },
				{ "Australia East", 238L },
				{ "Australia Southeast", 235L },
				{ "Austria East", 14L },
				{ "Brazil South", 186L },
				{ "Brazil Southeast", 186L },
				{ "Canada Central", 92L },
				{ "Canada East", 102L },
				{ "Central India", 116L },
				{ "Central US", 101L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 184L },
				{ "East US", 80L },
				{ "East US 2", 80L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 100L },
				{ "France South", 11L },
				{ "Germany North", 20L },
				{ "Germany West Central", 10L },
				{ "Indonesia Central", 150L },
				{ "Israel Central", 108L },
				{ "Italy North", 14L },
				{ "Japan East", 218L },
				{ "Japan West", 218L },
				{ "Jio India Central", 116L },
				{ "Jio India West", 116L },
				{ "Korea Central", 212L },
				{ "Korea South", 206L },
				{ "Malaysia South", 150L },
				{ "Mexico Central", 140L },
				{ "New Zealand North", 242L },
				{ "North Central US", 96L },
				{ "North Europe", 27L },
				{ "Norway East", 30L },
				{ "Norway West", 24L },
				{ "Poland Central", 10L },
				{ "Qatar Central", 108L },
				{ "South Africa North", 166L },
				{ "South Africa West", 150L },
				{ "South Central US", 106L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 150L },
				{ "South India", 128L },
				{ "Spain Central", 0L },
				{ "Sweden Central", 30L },
				{ "Sweden South", 30L },
				{ "Switzerland North", 14L },
				{ "Switzerland West", 10L },
				{ "Taiwan North", 184L },
				{ "Taiwan Northwest", 184L },
				{ "UAE Central", 108L },
				{ "UAE North", 112L },
				{ "UK South", 7L },
				{ "UK West", 8L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 119L },
				{ "West Europe", 10L },
				{ "West India", 118L },
				{ "West US", 138L },
				{ "West US 2", 140L },
				{ "West US 3", 140L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Sweden Central",
			new Dictionary<string, long>
			{
				{ "Australia Central", 272L },
				{ "Australia Central 2", 272L },
				{ "Australia East", 266L },
				{ "Australia Southeast", 264L },
				{ "Austria East", 32L },
				{ "Brazil South", 208L },
				{ "Brazil Southeast", 208L },
				{ "Canada Central", 114L },
				{ "Canada East", 124L },
				{ "Central India", 146L },
				{ "Central US", 128L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 214L },
				{ "East US", 101L },
				{ "East US 2", 104L },
				{ "East US SLV", 255L },
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 30L },
				{ "France South", 42L },
				{ "Germany North", 20L },
				{ "Germany West Central", 26L },
				{ "Indonesia Central", 180L },
				{ "Israel Central", 129L },
				{ "Italy North", 32L },
				{ "Japan East", 254L },
				{ "Japan West", 248L },
				{ "Jio India Central", 146L },
				{ "Jio India West", 146L },
				{ "Korea Central", 242L },
				{ "Korea South", 235L },
				{ "Malaysia South", 180L },
				{ "Mexico Central", 164L },
				{ "New Zealand North", 272L },
				{ "North Central US", 122L },
				{ "North Europe", 40L },
				{ "Norway East", 10L },
				{ "Norway West", 10L },
				{ "Poland Central", 22L },
				{ "Qatar Central", 138L },
				{ "South Africa North", 186L },
				{ "South Africa West", 170L },
				{ "South Central US", 132L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 180L },
				{ "South India", 157L },
				{ "Spain Central", 30L },
				{ "Sweden Central", 0L },
				{ "Sweden South", 10L },
				{ "Switzerland North", 32L },
				{ "Switzerland West", 36L },
				{ "Taiwan North", 214L },
				{ "Taiwan Northwest", 214L },
				{ "UAE Central", 138L },
				{ "UAE North", 140L },
				{ "UK South", 28L },
				{ "UK West", 30L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 142L },
				{ "West Europe", 22L },
				{ "West India", 146L },
				{ "West US", 163L },
				{ "West US 2", 164L },
				{ "West US 3", 164L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Sweden South",
			new Dictionary<string, long>
			{
				{ "Australia Central", 272L },
				{ "Australia Central 2", 272L },
				{ "Australia East", 266L },
				{ "Australia Southeast", 264L },
				{ "Austria East", 255L },
				{ "Brazil South", 208L },
				{ "Brazil Southeast", 208L },
				{ "Canada Central", 114L },
				{ "Canada East", 124L },
				{ "Central India", 146L },
				{ "Central US", 128L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 214L },
				{ "East US", 101L },
				{ "East US 2", 104L },
				{ "East US SLV", 255L },
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 30L },
				{ "France South", 42L },
				{ "Germany North", 20L },
				{ "Germany West Central", 26L },
				{ "Indonesia Central", 180L },
				{ "Israel Central", 129L },
				{ "Italy North", 255L },
				{ "Japan East", 254L },
				{ "Japan West", 248L },
				{ "Jio India Central", 146L },
				{ "Jio India West", 146L },
				{ "Korea Central", 242L },
				{ "Korea South", 235L },
				{ "Malaysia South", 180L },
				{ "Mexico Central", 164L },
				{ "New Zealand North", 272L },
				{ "North Central US", 122L },
				{ "North Europe", 40L },
				{ "Norway East", 10L },
				{ "Norway West", 10L },
				{ "Poland Central", 22L },
				{ "Qatar Central", 138L },
				{ "South Africa North", 186L },
				{ "South Africa West", 170L },
				{ "South Central US", 132L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 180L },
				{ "South India", 157L },
				{ "Spain Central", 30L },
				{ "Sweden Central", 5L },
				{ "Sweden South", 0L },
				{ "Switzerland North", 32L },
				{ "Switzerland West", 36L },
				{ "Taiwan North", 214L },
				{ "Taiwan Northwest", 214L },
				{ "UAE Central", 138L },
				{ "UAE North", 140L },
				{ "UK South", 28L },
				{ "UK West", 28L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 142L },
				{ "West Europe", 22L },
				{ "West India", 146L },
				{ "West US", 163L },
				{ "West US 2", 164L },
				{ "West US 3", 164L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Switzerland North",
			new Dictionary<string, long>
			{
				{ "Australia Central", 240L },
				{ "Australia Central 2", 240L },
				{ "Australia East", 236L },
				{ "Australia Southeast", 232L },
				{ "Austria East", 100L },
				{ "Brazil South", 198L },
				{ "Brazil Southeast", 198L },
				{ "Canada Central", 104L },
				{ "Canada East", 114L },
				{ "Central India", 112L },
				{ "Central US", 112L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 182L },
				{ "East US", 92L },
				{ "East US 2", 94L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 14L },
				{ "France South", 10L },
				{ "Germany North", 16L },
				{ "Germany West Central", 6L },
				{ "Indonesia Central", 148L },
				{ "Israel Central", 106L },
				{ "Italy North", 100L },
				{ "Japan East", 216L },
				{ "Japan West", 216L },
				{ "Jio India Central", 112L },
				{ "Jio India West", 112L },
				{ "Korea Central", 210L },
				{ "Korea South", 204L },
				{ "Malaysia South", 148L },
				{ "Mexico Central", 154L },
				{ "New Zealand North", 240L },
				{ "North Central US", 110L },
				{ "North Europe", 31L },
				{ "Norway East", 32L },
				{ "Norway West", 26L },
				{ "Poland Central", 12L },
				{ "Qatar Central", 106L },
				{ "South Africa North", 172L },
				{ "South Africa West", 162L },
				{ "South Central US", 118L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 148L },
				{ "South India", 126L },
				{ "Spain Central", 14L },
				{ "Sweden Central", 32L },
				{ "Sweden South", 255L },
				{ "Switzerland North", 0L },
				{ "Switzerland West", 4L },
				{ "Taiwan North", 182L },
				{ "Taiwan Northwest", 182L },
				{ "UAE Central", 106L },
				{ "UAE North", 108L },
				{ "UK South", 20L },
				{ "UK West", 22L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 134L },
				{ "West Europe", 12L },
				{ "West India", 116L },
				{ "West US", 150L },
				{ "West US 2", 154L },
				{ "West US 3", 154L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Switzerland West",
			new Dictionary<string, long>
			{
				{ "Australia Central", 236L },
				{ "Australia Central 2", 236L },
				{ "Australia East", 232L },
				{ "Australia Southeast", 228L },
				{ "Austria East", 4L },
				{ "Brazil South", 202L },
				{ "Brazil Southeast", 202L },
				{ "Canada Central", 108L },
				{ "Canada East", 118L },
				{ "Central India", 110L },
				{ "Central US", 110L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 178L },
				{ "East US", 88L },
				{ "East US 2", 88L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 10L },
				{ "France South", 8L },
				{ "Germany North", 20L },
				{ "Germany West Central", 10L },
				{ "Indonesia Central", 144L },
				{ "Israel Central", 102L },
				{ "Italy North", 4L },
				{ "Japan East", 212L },
				{ "Japan West", 210L },
				{ "Jio India Central", 110L },
				{ "Jio India West", 110L },
				{ "Korea Central", 206L },
				{ "Korea South", 200L },
				{ "Malaysia South", 144L },
				{ "Mexico Central", 146L },
				{ "New Zealand North", 236L },
				{ "North Central US", 106L },
				{ "North Europe", 30L },
				{ "Norway East", 36L },
				{ "Norway West", 30L },
				{ "Poland Central", 16L },
				{ "Qatar Central", 102L },
				{ "South Africa North", 168L },
				{ "South Africa West", 158L },
				{ "South Central US", 116L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 144L },
				{ "South India", 122L },
				{ "Spain Central", 10L },
				{ "Sweden Central", 36L },
				{ "Sweden South", 36L },
				{ "Switzerland North", 4L },
				{ "Switzerland West", 0L },
				{ "Taiwan North", 178L },
				{ "Taiwan Northwest", 178L },
				{ "UAE Central", 102L },
				{ "UAE North", 106L },
				{ "UK South", 16L },
				{ "UK West", 18L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 126L },
				{ "West Europe", 16L },
				{ "West India", 112L },
				{ "West US", 147L },
				{ "West US 2", 146L },
				{ "West US 3", 146L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Taiwan North",
			new Dictionary<string, long>
			{
				{ "Australia Central", 120L },
				{ "Australia Central 2", 120L },
				{ "Australia East", 116L },
				{ "Australia Southeast", 118L },
				{ "Austria East", 182L },
				{ "Brazil South", 320L },
				{ "Brazil Southeast", 320L },
				{ "Canada Central", 196L },
				{ "Canada East", 206L },
				{ "Central India", 86L },
				{ "Central US", 176L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 100L },
				{ "East US", 199L },
				{ "East US 2", 210L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 184L },
				{ "France South", 172L },
				{ "Germany North", 196L },
				{ "Germany West Central", 188L },
				{ "Indonesia Central", 33L },
				{ "Israel Central", 110L },
				{ "Italy North", 182L },
				{ "Japan East", 50L },
				{ "Japan West", 50L },
				{ "Jio India Central", 86L },
				{ "Jio India West", 86L },
				{ "Korea Central", 56L },
				{ "Korea South", 60L },
				{ "Malaysia South", 33L },
				{ "Mexico Central", 140L },
				{ "New Zealand North", 120L },
				{ "North Central US", 184L },
				{ "North Europe", 205L },
				{ "Norway East", 214L },
				{ "Norway West", 206L },
				{ "Poland Central", 193L },
				{ "Qatar Central", 110L },
				{ "South Africa North", 332L },
				{ "South Africa West", 336L },
				{ "South Central US", 182L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 33L },
				{ "South India", 66L },
				{ "Spain Central", 184L },
				{ "Sweden Central", 214L },
				{ "Sweden South", 214L },
				{ "Switzerland North", 182L },
				{ "Switzerland West", 178L },
				{ "Taiwan North", 0L },
				{ "Taiwan Northwest", 100L },
				{ "UAE Central", 110L },
				{ "UAE North", 109L },
				{ "UK South", 192L },
				{ "UK West", 194L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 162L },
				{ "West Europe", 193L },
				{ "West India", 84L },
				{ "West US", 148L },
				{ "West US 2", 140L },
				{ "West US 3", 140L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Taiwan Northwest",
			new Dictionary<string, long>
			{
				{ "Australia Central", 120L },
				{ "Australia Central 2", 120L },
				{ "Australia East", 116L },
				{ "Australia Southeast", 118L },
				{ "Austria East", 182L },
				{ "Brazil South", 320L },
				{ "Brazil Southeast", 320L },
				{ "Canada Central", 196L },
				{ "Canada East", 206L },
				{ "Central India", 86L },
				{ "Central US", 176L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 100L },
				{ "East US", 199L },
				{ "East US 2", 210L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 184L },
				{ "France South", 172L },
				{ "Germany North", 196L },
				{ "Germany West Central", 188L },
				{ "Indonesia Central", 33L },
				{ "Israel Central", 110L },
				{ "Italy North", 182L },
				{ "Japan East", 50L },
				{ "Japan West", 50L },
				{ "Jio India Central", 86L },
				{ "Jio India West", 86L },
				{ "Korea Central", 56L },
				{ "Korea South", 60L },
				{ "Malaysia South", 33L },
				{ "Mexico Central", 140L },
				{ "New Zealand North", 120L },
				{ "North Central US", 184L },
				{ "North Europe", 205L },
				{ "Norway East", 214L },
				{ "Norway West", 206L },
				{ "Poland Central", 193L },
				{ "Qatar Central", 110L },
				{ "South Africa North", 332L },
				{ "South Africa West", 336L },
				{ "South Central US", 182L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 33L },
				{ "South India", 66L },
				{ "Spain Central", 184L },
				{ "Sweden Central", 214L },
				{ "Sweden South", 214L },
				{ "Switzerland North", 182L },
				{ "Switzerland West", 178L },
				{ "Taiwan North", 100L },
				{ "Taiwan Northwest", 0L },
				{ "UAE Central", 110L },
				{ "UAE North", 109L },
				{ "UK South", 192L },
				{ "UK West", 194L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 162L },
				{ "West Europe", 193L },
				{ "West India", 84L },
				{ "West US", 148L },
				{ "West US 2", 140L },
				{ "West US 3", 140L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"UAE Central",
			new Dictionary<string, long>
			{
				{ "Australia Central", 168L },
				{ "Australia Central 2", 168L },
				{ "Australia East", 164L },
				{ "Australia Southeast", 160L },
				{ "Austria East", 106L },
				{ "Brazil South", 296L },
				{ "Brazil Southeast", 296L },
				{ "Canada Central", 201L },
				{ "Canada East", 210L },
				{ "Central India", 30L },
				{ "Central US", 216L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 110L },
				{ "East US", 189L },
				{ "East US 2", 190L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 108L },
				{ "France South", 96L },
				{ "Germany North", 120L },
				{ "Germany West Central", 112L },
				{ "Indonesia Central", 76L },
				{ "Israel Central", 100L },
				{ "Italy North", 106L },
				{ "Japan East", 144L },
				{ "Japan West", 149L },
				{ "Jio India Central", 30L },
				{ "Jio India West", 30L },
				{ "Korea Central", 138L },
				{ "Korea South", 132L },
				{ "Malaysia South", 76L },
				{ "Mexico Central", 236L },
				{ "New Zealand North", 168L },
				{ "North Central US", 209L },
				{ "North Europe", 129L },
				{ "Norway East", 138L },
				{ "Norway West", 130L },
				{ "Poland Central", 118L },
				{ "Qatar Central", 4L },
				{ "South Africa North", 256L },
				{ "South Africa West", 260L },
				{ "South Central US", 218L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 76L },
				{ "South India", 46L },
				{ "Spain Central", 108L },
				{ "Sweden Central", 129L },
				{ "Sweden South", 129L },
				{ "Switzerland North", 106L },
				{ "Switzerland West", 102L },
				{ "Taiwan North", 110L },
				{ "Taiwan Northwest", 110L },
				{ "UAE Central", 0L },
				{ "UAE North", 4L },
				{ "UK South", 116L },
				{ "UK West", 118L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 230L },
				{ "West Europe", 118L },
				{ "West India", 28L },
				{ "West US", 244L },
				{ "West US 2", 236L },
				{ "West US 3", 236L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"UAE North",
			new Dictionary<string, long>
			{
				{ "Australia Central", 168L },
				{ "Australia Central 2", 168L },
				{ "Australia East", 163L },
				{ "Australia Southeast", 160L },
				{ "Austria East", 108L },
				{ "Brazil South", 298L },
				{ "Brazil Southeast", 298L },
				{ "Canada Central", 204L },
				{ "Canada East", 214L },
				{ "Central India", 30L },
				{ "Central US", 218L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 109L },
				{ "East US", 192L },
				{ "East US 2", 194L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 112L },
				{ "France South", 98L },
				{ "Germany North", 124L },
				{ "Germany West Central", 114L },
				{ "Indonesia Central", 76L },
				{ "Israel Central", 4L },
				{ "Italy North", 108L },
				{ "Japan East", 144L },
				{ "Japan West", 148L },
				{ "Jio India Central", 30L },
				{ "Jio India West", 30L },
				{ "Korea Central", 138L },
				{ "Korea South", 130L },
				{ "Malaysia South", 76L },
				{ "Mexico Central", 236L },
				{ "New Zealand North", 168L },
				{ "North Central US", 212L },
				{ "North Europe", 132L },
				{ "Norway East", 140L },
				{ "Norway West", 134L },
				{ "Poland Central", 120L },
				{ "Qatar Central", 4L },
				{ "South Africa North", 260L },
				{ "South Africa West", 264L },
				{ "South Central US", 220L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 76L },
				{ "South India", 46L },
				{ "Spain Central", 112L },
				{ "Sweden Central", 140L },
				{ "Sweden South", 140L },
				{ "Switzerland North", 108L },
				{ "Switzerland West", 106L },
				{ "Taiwan North", 109L },
				{ "Taiwan Northwest", 109L },
				{ "UAE Central", 4L },
				{ "UAE North", 0L },
				{ "UK South", 120L },
				{ "UK West", 122L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 234L },
				{ "West Europe", 120L },
				{ "West India", 28L },
				{ "West US", 244L },
				{ "West US 2", 236L },
				{ "West US 3", 236L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"UK South",
			new Dictionary<string, long>
			{
				{ "Australia Central", 250L },
				{ "Australia Central 2", 248L },
				{ "Australia East", 246L },
				{ "Australia Southeast", 242L },
				{ "Austria East", 20L },
				{ "Brazil South", 181L },
				{ "Brazil Southeast", 181L },
				{ "Canada Central", 86L },
				{ "Canada East", 96L },
				{ "Central India", 122L },
				{ "Central US", 96L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 192L },
				{ "East US", 74L },
				{ "East US 2", 78L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 7L },
				{ "France South", 16L },
				{ "Germany North", 20L },
				{ "Germany West Central", 14L },
				{ "Indonesia Central", 158L },
				{ "Israel Central", 116L },
				{ "Italy North", 20L },
				{ "Japan East", 226L },
				{ "Japan West", 225L },
				{ "Jio India Central", 122L },
				{ "Jio India West", 122L },
				{ "Korea Central", 220L },
				{ "Korea South", 214L },
				{ "Malaysia South", 158L },
				{ "Mexico Central", 130L },
				{ "New Zealand North", 250L },
				{ "North Central US", 90L },
				{ "North Europe", 21L },
				{ "Norway East", 28L },
				{ "Norway West", 22L },
				{ "Poland Central", 10L },
				{ "Qatar Central", 116L },
				{ "South Africa North", 160L },
				{ "South Africa West", 144L },
				{ "South Central US", 104L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 158L },
				{ "South India", 134L },
				{ "Spain Central", 7L },
				{ "Sweden Central", 21L },
				{ "Sweden South", 21L },
				{ "Switzerland North", 20L },
				{ "Switzerland West", 16L },
				{ "Taiwan North", 192L },
				{ "Taiwan Northwest", 192L },
				{ "UAE Central", 116L },
				{ "UAE North", 120L },
				{ "UK South", 0L },
				{ "UK West", 4L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 110L },
				{ "West Europe", 10L },
				{ "West India", 126L },
				{ "West US", 136L },
				{ "West US 2", 130L },
				{ "West US 3", 130L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"UK West",
			new Dictionary<string, long>
			{
				{ "Australia Central", 252L },
				{ "Australia Central 2", 252L },
				{ "Australia East", 248L },
				{ "Australia Southeast", 245L },
				{ "Austria East", 22L },
				{ "Brazil South", 184L },
				{ "Brazil Southeast", 184L },
				{ "Canada Central", 90L },
				{ "Canada East", 98L },
				{ "Central India", 126L },
				{ "Central US", 96L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 194L },
				{ "East US", 76L },
				{ "East US 2", 80L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 8L },
				{ "France South", 18L },
				{ "Germany North", 22L },
				{ "Germany West Central", 18L },
				{ "Indonesia Central", 161L },
				{ "Israel Central", 118L },
				{ "Italy North", 22L },
				{ "Japan East", 230L },
				{ "Japan West", 228L },
				{ "Jio India Central", 126L },
				{ "Jio India West", 126L },
				{ "Korea Central", 222L },
				{ "Korea South", 216L },
				{ "Malaysia South", 161L },
				{ "Mexico Central", 132L },
				{ "New Zealand North", 252L },
				{ "North Central US", 92L },
				{ "North Europe", 17L },
				{ "Norway East", 30L },
				{ "Norway West", 24L },
				{ "Poland Central", 12L },
				{ "Qatar Central", 118L },
				{ "South Africa North", 164L },
				{ "South Africa West", 146L },
				{ "South Central US", 106L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 161L },
				{ "South India", 138L },
				{ "Spain Central", 8L },
				{ "Sweden Central", 30L },
				{ "Sweden South", 30L },
				{ "Switzerland North", 22L },
				{ "Switzerland West", 18L },
				{ "Taiwan North", 194L },
				{ "Taiwan Northwest", 194L },
				{ "UAE Central", 118L },
				{ "UAE North", 122L },
				{ "UK South", 4L },
				{ "UK West", 0L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 112L },
				{ "West Europe", 12L },
				{ "West India", 128L },
				{ "West US", 138L },
				{ "West US 2", 132L },
				{ "West US 3", 132L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"USDoD Central",
			new Dictionary<string, long>
			{
				{
					"Australia Central",
					long.MaxValue
				},
				{
					"Australia Central 2",
					long.MaxValue
				},
				{
					"Australia East",
					long.MaxValue
				},
				{
					"Australia Southeast",
					long.MaxValue
				},
				{
					"Austria East",
					long.MaxValue
				},
				{
					"Brazil South",
					long.MaxValue
				},
				{
					"Brazil Southeast",
					long.MaxValue
				},
				{
					"Canada Central",
					long.MaxValue
				},
				{
					"Canada East",
					long.MaxValue
				},
				{
					"Central India",
					long.MaxValue
				},
				{
					"Central US",
					long.MaxValue
				},
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{
					"East Asia",
					long.MaxValue
				},
				{
					"East US",
					long.MaxValue
				},
				{
					"East US 2",
					long.MaxValue
				},
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{
					"France Central",
					long.MaxValue
				},
				{
					"France South",
					long.MaxValue
				},
				{
					"Germany North",
					long.MaxValue
				},
				{
					"Germany West Central",
					long.MaxValue
				},
				{
					"Indonesia Central",
					long.MaxValue
				},
				{
					"Israel Central",
					long.MaxValue
				},
				{
					"Italy North",
					long.MaxValue
				},
				{
					"Japan East",
					long.MaxValue
				},
				{
					"Japan West",
					long.MaxValue
				},
				{
					"Jio India Central",
					long.MaxValue
				},
				{
					"Jio India West",
					long.MaxValue
				},
				{
					"Korea Central",
					long.MaxValue
				},
				{
					"Korea South",
					long.MaxValue
				},
				{
					"Malaysia South",
					long.MaxValue
				},
				{
					"Mexico Central",
					long.MaxValue
				},
				{
					"New Zealand North",
					long.MaxValue
				},
				{
					"North Central US",
					long.MaxValue
				},
				{
					"North Europe",
					long.MaxValue
				},
				{
					"Norway East",
					long.MaxValue
				},
				{
					"Norway West",
					long.MaxValue
				},
				{
					"Poland Central",
					long.MaxValue
				},
				{
					"Qatar Central",
					long.MaxValue
				},
				{
					"South Africa North",
					long.MaxValue
				},
				{
					"South Africa West",
					long.MaxValue
				},
				{
					"South Central US",
					long.MaxValue
				},
				{
					"South Central US STG",
					long.MaxValue
				},
				{
					"Southeast Asia",
					long.MaxValue
				},
				{
					"South India",
					long.MaxValue
				},
				{
					"Spain Central",
					long.MaxValue
				},
				{
					"Sweden Central",
					long.MaxValue
				},
				{
					"Sweden South",
					long.MaxValue
				},
				{
					"Switzerland North",
					long.MaxValue
				},
				{
					"Switzerland West",
					long.MaxValue
				},
				{
					"Taiwan North",
					long.MaxValue
				},
				{
					"Taiwan Northwest",
					long.MaxValue
				},
				{
					"UAE Central",
					long.MaxValue
				},
				{
					"UAE North",
					long.MaxValue
				},
				{
					"UK South",
					long.MaxValue
				},
				{
					"UK West",
					long.MaxValue
				},
				{ "USDoD Central", 0L },
				{ "USDoD East", 44L },
				{ "USGov Arizona", 42L },
				{ "USGov Texas", 24L },
				{ "USGov Virginia", 44L },
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{
					"West Central US",
					long.MaxValue
				},
				{
					"West Europe",
					long.MaxValue
				},
				{
					"West India",
					long.MaxValue
				},
				{
					"West US",
					long.MaxValue
				},
				{
					"West US 2",
					long.MaxValue
				},
				{
					"West US 3",
					long.MaxValue
				},
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"USDoD East",
			new Dictionary<string, long>
			{
				{
					"Australia Central",
					long.MaxValue
				},
				{
					"Australia Central 2",
					long.MaxValue
				},
				{
					"Australia East",
					long.MaxValue
				},
				{
					"Australia Southeast",
					long.MaxValue
				},
				{
					"Austria East",
					long.MaxValue
				},
				{
					"Brazil South",
					long.MaxValue
				},
				{
					"Brazil Southeast",
					long.MaxValue
				},
				{
					"Canada Central",
					long.MaxValue
				},
				{
					"Canada East",
					long.MaxValue
				},
				{
					"Central India",
					long.MaxValue
				},
				{
					"Central US",
					long.MaxValue
				},
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{
					"East Asia",
					long.MaxValue
				},
				{
					"East US",
					long.MaxValue
				},
				{
					"East US 2",
					long.MaxValue
				},
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{
					"France Central",
					long.MaxValue
				},
				{
					"France South",
					long.MaxValue
				},
				{
					"Germany North",
					long.MaxValue
				},
				{
					"Germany West Central",
					long.MaxValue
				},
				{
					"Indonesia Central",
					long.MaxValue
				},
				{
					"Israel Central",
					long.MaxValue
				},
				{
					"Italy North",
					long.MaxValue
				},
				{
					"Japan East",
					long.MaxValue
				},
				{
					"Japan West",
					long.MaxValue
				},
				{
					"Jio India Central",
					long.MaxValue
				},
				{
					"Jio India West",
					long.MaxValue
				},
				{
					"Korea Central",
					long.MaxValue
				},
				{
					"Korea South",
					long.MaxValue
				},
				{
					"Malaysia South",
					long.MaxValue
				},
				{
					"Mexico Central",
					long.MaxValue
				},
				{
					"New Zealand North",
					long.MaxValue
				},
				{
					"North Central US",
					long.MaxValue
				},
				{
					"North Europe",
					long.MaxValue
				},
				{
					"Norway East",
					long.MaxValue
				},
				{
					"Norway West",
					long.MaxValue
				},
				{
					"Poland Central",
					long.MaxValue
				},
				{
					"Qatar Central",
					long.MaxValue
				},
				{
					"South Africa North",
					long.MaxValue
				},
				{
					"South Africa West",
					long.MaxValue
				},
				{
					"South Central US",
					long.MaxValue
				},
				{
					"South Central US STG",
					long.MaxValue
				},
				{
					"Southeast Asia",
					long.MaxValue
				},
				{
					"South India",
					long.MaxValue
				},
				{
					"Spain Central",
					long.MaxValue
				},
				{
					"Sweden Central",
					long.MaxValue
				},
				{
					"Sweden South",
					long.MaxValue
				},
				{
					"Switzerland North",
					long.MaxValue
				},
				{
					"Switzerland West",
					long.MaxValue
				},
				{
					"Taiwan North",
					long.MaxValue
				},
				{
					"Taiwan Northwest",
					long.MaxValue
				},
				{
					"UAE Central",
					long.MaxValue
				},
				{
					"UAE North",
					long.MaxValue
				},
				{
					"UK South",
					long.MaxValue
				},
				{
					"UK West",
					long.MaxValue
				},
				{ "USDoD Central", 44L },
				{ "USDoD East", 0L },
				{ "USGov Arizona", 45L },
				{ "USGov Texas", 27L },
				{ "USGov Virginia", 2L },
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{
					"West Central US",
					long.MaxValue
				},
				{
					"West Europe",
					long.MaxValue
				},
				{
					"West India",
					long.MaxValue
				},
				{
					"West US",
					long.MaxValue
				},
				{
					"West US 2",
					long.MaxValue
				},
				{
					"West US 3",
					long.MaxValue
				},
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"USGov Arizona",
			new Dictionary<string, long>
			{
				{
					"Australia Central",
					long.MaxValue
				},
				{
					"Australia Central 2",
					long.MaxValue
				},
				{
					"Australia East",
					long.MaxValue
				},
				{
					"Australia Southeast",
					long.MaxValue
				},
				{
					"Austria East",
					long.MaxValue
				},
				{
					"Brazil South",
					long.MaxValue
				},
				{
					"Brazil Southeast",
					long.MaxValue
				},
				{
					"Canada Central",
					long.MaxValue
				},
				{
					"Canada East",
					long.MaxValue
				},
				{
					"Central India",
					long.MaxValue
				},
				{
					"Central US",
					long.MaxValue
				},
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{
					"East Asia",
					long.MaxValue
				},
				{
					"East US",
					long.MaxValue
				},
				{
					"East US 2",
					long.MaxValue
				},
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{
					"France Central",
					long.MaxValue
				},
				{
					"France South",
					long.MaxValue
				},
				{
					"Germany North",
					long.MaxValue
				},
				{
					"Germany West Central",
					long.MaxValue
				},
				{
					"Indonesia Central",
					long.MaxValue
				},
				{
					"Israel Central",
					long.MaxValue
				},
				{
					"Italy North",
					long.MaxValue
				},
				{
					"Japan East",
					long.MaxValue
				},
				{
					"Japan West",
					long.MaxValue
				},
				{
					"Jio India Central",
					long.MaxValue
				},
				{
					"Jio India West",
					long.MaxValue
				},
				{
					"Korea Central",
					long.MaxValue
				},
				{
					"Korea South",
					long.MaxValue
				},
				{
					"Malaysia South",
					long.MaxValue
				},
				{
					"Mexico Central",
					long.MaxValue
				},
				{
					"New Zealand North",
					long.MaxValue
				},
				{
					"North Central US",
					long.MaxValue
				},
				{
					"North Europe",
					long.MaxValue
				},
				{
					"Norway East",
					long.MaxValue
				},
				{
					"Norway West",
					long.MaxValue
				},
				{
					"Poland Central",
					long.MaxValue
				},
				{
					"Qatar Central",
					long.MaxValue
				},
				{
					"South Africa North",
					long.MaxValue
				},
				{
					"South Africa West",
					long.MaxValue
				},
				{
					"South Central US",
					long.MaxValue
				},
				{
					"South Central US STG",
					long.MaxValue
				},
				{
					"Southeast Asia",
					long.MaxValue
				},
				{
					"South India",
					long.MaxValue
				},
				{
					"Spain Central",
					long.MaxValue
				},
				{
					"Sweden Central",
					long.MaxValue
				},
				{
					"Sweden South",
					long.MaxValue
				},
				{
					"Switzerland North",
					long.MaxValue
				},
				{
					"Switzerland West",
					long.MaxValue
				},
				{
					"Taiwan North",
					long.MaxValue
				},
				{
					"Taiwan Northwest",
					long.MaxValue
				},
				{
					"UAE Central",
					long.MaxValue
				},
				{
					"UAE North",
					long.MaxValue
				},
				{
					"UK South",
					long.MaxValue
				},
				{
					"UK West",
					long.MaxValue
				},
				{ "USDoD Central", 42L },
				{ "USDoD East", 46L },
				{ "USGov Arizona", 0L },
				{ "USGov Texas", 20L },
				{ "USGov Virginia", 45L },
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{
					"West Central US",
					long.MaxValue
				},
				{
					"West Europe",
					long.MaxValue
				},
				{
					"West India",
					long.MaxValue
				},
				{
					"West US",
					long.MaxValue
				},
				{
					"West US 2",
					long.MaxValue
				},
				{
					"West US 3",
					long.MaxValue
				},
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"USGov Texas",
			new Dictionary<string, long>
			{
				{
					"Australia Central",
					long.MaxValue
				},
				{
					"Australia Central 2",
					long.MaxValue
				},
				{
					"Australia East",
					long.MaxValue
				},
				{
					"Australia Southeast",
					long.MaxValue
				},
				{
					"Austria East",
					long.MaxValue
				},
				{
					"Brazil South",
					long.MaxValue
				},
				{
					"Brazil Southeast",
					long.MaxValue
				},
				{
					"Canada Central",
					long.MaxValue
				},
				{
					"Canada East",
					long.MaxValue
				},
				{
					"Central India",
					long.MaxValue
				},
				{
					"Central US",
					long.MaxValue
				},
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{
					"East Asia",
					long.MaxValue
				},
				{
					"East US",
					long.MaxValue
				},
				{
					"East US 2",
					long.MaxValue
				},
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{
					"France Central",
					long.MaxValue
				},
				{
					"France South",
					long.MaxValue
				},
				{
					"Germany North",
					long.MaxValue
				},
				{
					"Germany West Central",
					long.MaxValue
				},
				{
					"Indonesia Central",
					long.MaxValue
				},
				{
					"Israel Central",
					long.MaxValue
				},
				{
					"Italy North",
					long.MaxValue
				},
				{
					"Japan East",
					long.MaxValue
				},
				{
					"Japan West",
					long.MaxValue
				},
				{
					"Jio India Central",
					long.MaxValue
				},
				{
					"Jio India West",
					long.MaxValue
				},
				{
					"Korea Central",
					long.MaxValue
				},
				{
					"Korea South",
					long.MaxValue
				},
				{
					"Malaysia South",
					long.MaxValue
				},
				{
					"Mexico Central",
					long.MaxValue
				},
				{
					"New Zealand North",
					long.MaxValue
				},
				{
					"North Central US",
					long.MaxValue
				},
				{
					"North Europe",
					long.MaxValue
				},
				{
					"Norway East",
					long.MaxValue
				},
				{
					"Norway West",
					long.MaxValue
				},
				{
					"Poland Central",
					long.MaxValue
				},
				{
					"Qatar Central",
					long.MaxValue
				},
				{
					"South Africa North",
					long.MaxValue
				},
				{
					"South Africa West",
					long.MaxValue
				},
				{
					"South Central US",
					long.MaxValue
				},
				{
					"South Central US STG",
					long.MaxValue
				},
				{
					"Southeast Asia",
					long.MaxValue
				},
				{
					"South India",
					long.MaxValue
				},
				{
					"Spain Central",
					long.MaxValue
				},
				{
					"Sweden Central",
					long.MaxValue
				},
				{
					"Sweden South",
					long.MaxValue
				},
				{
					"Switzerland North",
					long.MaxValue
				},
				{
					"Switzerland West",
					long.MaxValue
				},
				{
					"Taiwan North",
					long.MaxValue
				},
				{
					"Taiwan Northwest",
					long.MaxValue
				},
				{
					"UAE Central",
					long.MaxValue
				},
				{
					"UAE North",
					long.MaxValue
				},
				{
					"UK South",
					long.MaxValue
				},
				{
					"UK West",
					long.MaxValue
				},
				{ "USDoD Central", 24L },
				{ "USDoD East", 28L },
				{ "USGov Arizona", 20L },
				{ "USGov Texas", 0L },
				{ "USGov Virginia", 35L },
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{
					"West Central US",
					long.MaxValue
				},
				{
					"West Europe",
					long.MaxValue
				},
				{
					"West India",
					long.MaxValue
				},
				{
					"West US",
					long.MaxValue
				},
				{
					"West US 2",
					long.MaxValue
				},
				{
					"West US 3",
					long.MaxValue
				},
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"USGov Virginia",
			new Dictionary<string, long>
			{
				{
					"Australia Central",
					long.MaxValue
				},
				{
					"Australia Central 2",
					long.MaxValue
				},
				{
					"Australia East",
					long.MaxValue
				},
				{
					"Australia Southeast",
					long.MaxValue
				},
				{
					"Austria East",
					long.MaxValue
				},
				{
					"Brazil South",
					long.MaxValue
				},
				{
					"Brazil Southeast",
					long.MaxValue
				},
				{
					"Canada Central",
					long.MaxValue
				},
				{
					"Canada East",
					long.MaxValue
				},
				{
					"Central India",
					long.MaxValue
				},
				{
					"Central US",
					long.MaxValue
				},
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{
					"East Asia",
					long.MaxValue
				},
				{
					"East US",
					long.MaxValue
				},
				{
					"East US 2",
					long.MaxValue
				},
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{
					"France Central",
					long.MaxValue
				},
				{
					"France South",
					long.MaxValue
				},
				{
					"Germany North",
					long.MaxValue
				},
				{
					"Germany West Central",
					long.MaxValue
				},
				{
					"Indonesia Central",
					long.MaxValue
				},
				{
					"Israel Central",
					long.MaxValue
				},
				{
					"Italy North",
					long.MaxValue
				},
				{
					"Japan East",
					long.MaxValue
				},
				{
					"Japan West",
					long.MaxValue
				},
				{
					"Jio India Central",
					long.MaxValue
				},
				{
					"Jio India West",
					long.MaxValue
				},
				{
					"Korea Central",
					long.MaxValue
				},
				{
					"Korea South",
					long.MaxValue
				},
				{
					"Malaysia South",
					long.MaxValue
				},
				{
					"Mexico Central",
					long.MaxValue
				},
				{
					"New Zealand North",
					long.MaxValue
				},
				{
					"North Central US",
					long.MaxValue
				},
				{
					"North Europe",
					long.MaxValue
				},
				{
					"Norway East",
					long.MaxValue
				},
				{
					"Norway West",
					long.MaxValue
				},
				{
					"Poland Central",
					long.MaxValue
				},
				{
					"Qatar Central",
					long.MaxValue
				},
				{
					"South Africa North",
					long.MaxValue
				},
				{
					"South Africa West",
					long.MaxValue
				},
				{
					"South Central US",
					long.MaxValue
				},
				{
					"South Central US STG",
					long.MaxValue
				},
				{
					"Southeast Asia",
					long.MaxValue
				},
				{
					"South India",
					long.MaxValue
				},
				{
					"Spain Central",
					long.MaxValue
				},
				{
					"Sweden Central",
					long.MaxValue
				},
				{
					"Sweden South",
					long.MaxValue
				},
				{
					"Switzerland North",
					long.MaxValue
				},
				{
					"Switzerland West",
					long.MaxValue
				},
				{
					"Taiwan North",
					long.MaxValue
				},
				{
					"Taiwan Northwest",
					long.MaxValue
				},
				{
					"UAE Central",
					long.MaxValue
				},
				{
					"UAE North",
					long.MaxValue
				},
				{
					"UK South",
					long.MaxValue
				},
				{
					"UK West",
					long.MaxValue
				},
				{ "USDoD Central", 44L },
				{ "USDoD East", 2L },
				{ "USGov Arizona", 45L },
				{ "USGov Texas", 35L },
				{ "USGov Virginia", 0L },
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{
					"West Central US",
					long.MaxValue
				},
				{
					"West Europe",
					long.MaxValue
				},
				{
					"West India",
					long.MaxValue
				},
				{
					"West US",
					long.MaxValue
				},
				{
					"West US 2",
					long.MaxValue
				},
				{
					"West US 3",
					long.MaxValue
				},
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"USNat East",
			new Dictionary<string, long>
			{
				{
					"Australia Central",
					long.MaxValue
				},
				{
					"Australia Central 2",
					long.MaxValue
				},
				{
					"Australia East",
					long.MaxValue
				},
				{
					"Australia Southeast",
					long.MaxValue
				},
				{
					"Austria East",
					long.MaxValue
				},
				{
					"Brazil South",
					long.MaxValue
				},
				{
					"Brazil Southeast",
					long.MaxValue
				},
				{
					"Canada Central",
					long.MaxValue
				},
				{
					"Canada East",
					long.MaxValue
				},
				{
					"Central India",
					long.MaxValue
				},
				{
					"Central US",
					long.MaxValue
				},
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{
					"East Asia",
					long.MaxValue
				},
				{
					"East US",
					long.MaxValue
				},
				{
					"East US 2",
					long.MaxValue
				},
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{
					"France Central",
					long.MaxValue
				},
				{
					"France South",
					long.MaxValue
				},
				{
					"Germany North",
					long.MaxValue
				},
				{
					"Germany West Central",
					long.MaxValue
				},
				{
					"Indonesia Central",
					long.MaxValue
				},
				{
					"Israel Central",
					long.MaxValue
				},
				{
					"Italy North",
					long.MaxValue
				},
				{
					"Japan East",
					long.MaxValue
				},
				{
					"Japan West",
					long.MaxValue
				},
				{
					"Jio India Central",
					long.MaxValue
				},
				{
					"Jio India West",
					long.MaxValue
				},
				{
					"Korea Central",
					long.MaxValue
				},
				{
					"Korea South",
					long.MaxValue
				},
				{
					"Malaysia South",
					long.MaxValue
				},
				{
					"Mexico Central",
					long.MaxValue
				},
				{
					"New Zealand North",
					long.MaxValue
				},
				{
					"North Central US",
					long.MaxValue
				},
				{
					"North Europe",
					long.MaxValue
				},
				{
					"Norway East",
					long.MaxValue
				},
				{
					"Norway West",
					long.MaxValue
				},
				{
					"Poland Central",
					long.MaxValue
				},
				{
					"Qatar Central",
					long.MaxValue
				},
				{
					"South Africa North",
					long.MaxValue
				},
				{
					"South Africa West",
					long.MaxValue
				},
				{
					"South Central US",
					long.MaxValue
				},
				{
					"South Central US STG",
					long.MaxValue
				},
				{
					"Southeast Asia",
					long.MaxValue
				},
				{
					"South India",
					long.MaxValue
				},
				{
					"Spain Central",
					long.MaxValue
				},
				{
					"Sweden Central",
					long.MaxValue
				},
				{
					"Sweden South",
					long.MaxValue
				},
				{
					"Switzerland North",
					long.MaxValue
				},
				{
					"Switzerland West",
					long.MaxValue
				},
				{
					"Taiwan North",
					long.MaxValue
				},
				{
					"Taiwan Northwest",
					long.MaxValue
				},
				{
					"UAE Central",
					long.MaxValue
				},
				{
					"UAE North",
					long.MaxValue
				},
				{
					"UK South",
					long.MaxValue
				},
				{
					"UK West",
					long.MaxValue
				},
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{ "USNat East", 0L },
				{ "USNat West", 255L },
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{
					"West Central US",
					long.MaxValue
				},
				{
					"West Europe",
					long.MaxValue
				},
				{
					"West India",
					long.MaxValue
				},
				{
					"West US",
					long.MaxValue
				},
				{
					"West US 2",
					long.MaxValue
				},
				{
					"West US 3",
					long.MaxValue
				},
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"USNat West",
			new Dictionary<string, long>
			{
				{
					"Australia Central",
					long.MaxValue
				},
				{
					"Australia Central 2",
					long.MaxValue
				},
				{
					"Australia East",
					long.MaxValue
				},
				{
					"Australia Southeast",
					long.MaxValue
				},
				{
					"Austria East",
					long.MaxValue
				},
				{
					"Brazil South",
					long.MaxValue
				},
				{
					"Brazil Southeast",
					long.MaxValue
				},
				{
					"Canada Central",
					long.MaxValue
				},
				{
					"Canada East",
					long.MaxValue
				},
				{
					"Central India",
					long.MaxValue
				},
				{
					"Central US",
					long.MaxValue
				},
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{
					"East Asia",
					long.MaxValue
				},
				{
					"East US",
					long.MaxValue
				},
				{
					"East US 2",
					long.MaxValue
				},
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{
					"France Central",
					long.MaxValue
				},
				{
					"France South",
					long.MaxValue
				},
				{
					"Germany North",
					long.MaxValue
				},
				{
					"Germany West Central",
					long.MaxValue
				},
				{
					"Indonesia Central",
					long.MaxValue
				},
				{
					"Israel Central",
					long.MaxValue
				},
				{
					"Italy North",
					long.MaxValue
				},
				{
					"Japan East",
					long.MaxValue
				},
				{
					"Japan West",
					long.MaxValue
				},
				{
					"Jio India Central",
					long.MaxValue
				},
				{
					"Jio India West",
					long.MaxValue
				},
				{
					"Korea Central",
					long.MaxValue
				},
				{
					"Korea South",
					long.MaxValue
				},
				{
					"Malaysia South",
					long.MaxValue
				},
				{
					"Mexico Central",
					long.MaxValue
				},
				{
					"New Zealand North",
					long.MaxValue
				},
				{
					"North Central US",
					long.MaxValue
				},
				{
					"North Europe",
					long.MaxValue
				},
				{
					"Norway East",
					long.MaxValue
				},
				{
					"Norway West",
					long.MaxValue
				},
				{
					"Poland Central",
					long.MaxValue
				},
				{
					"Qatar Central",
					long.MaxValue
				},
				{
					"South Africa North",
					long.MaxValue
				},
				{
					"South Africa West",
					long.MaxValue
				},
				{
					"South Central US",
					long.MaxValue
				},
				{
					"South Central US STG",
					long.MaxValue
				},
				{
					"Southeast Asia",
					long.MaxValue
				},
				{
					"South India",
					long.MaxValue
				},
				{
					"Spain Central",
					long.MaxValue
				},
				{
					"Sweden Central",
					long.MaxValue
				},
				{
					"Sweden South",
					long.MaxValue
				},
				{
					"Switzerland North",
					long.MaxValue
				},
				{
					"Switzerland West",
					long.MaxValue
				},
				{
					"Taiwan North",
					long.MaxValue
				},
				{
					"Taiwan Northwest",
					long.MaxValue
				},
				{
					"UAE Central",
					long.MaxValue
				},
				{
					"UAE North",
					long.MaxValue
				},
				{
					"UK South",
					long.MaxValue
				},
				{
					"UK West",
					long.MaxValue
				},
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{ "USNat East", 255L },
				{ "USNat West", 0L },
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{
					"West Central US",
					long.MaxValue
				},
				{
					"West Europe",
					long.MaxValue
				},
				{
					"West India",
					long.MaxValue
				},
				{
					"West US",
					long.MaxValue
				},
				{
					"West US 2",
					long.MaxValue
				},
				{
					"West US 3",
					long.MaxValue
				},
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"USSec East",
			new Dictionary<string, long>
			{
				{
					"Australia Central",
					long.MaxValue
				},
				{
					"Australia Central 2",
					long.MaxValue
				},
				{
					"Australia East",
					long.MaxValue
				},
				{
					"Australia Southeast",
					long.MaxValue
				},
				{
					"Austria East",
					long.MaxValue
				},
				{
					"Brazil South",
					long.MaxValue
				},
				{
					"Brazil Southeast",
					long.MaxValue
				},
				{
					"Canada Central",
					long.MaxValue
				},
				{
					"Canada East",
					long.MaxValue
				},
				{
					"Central India",
					long.MaxValue
				},
				{
					"Central US",
					long.MaxValue
				},
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{
					"East Asia",
					long.MaxValue
				},
				{
					"East US",
					long.MaxValue
				},
				{
					"East US 2",
					long.MaxValue
				},
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{
					"France Central",
					long.MaxValue
				},
				{
					"France South",
					long.MaxValue
				},
				{
					"Germany North",
					long.MaxValue
				},
				{
					"Germany West Central",
					long.MaxValue
				},
				{
					"Indonesia Central",
					long.MaxValue
				},
				{
					"Israel Central",
					long.MaxValue
				},
				{
					"Italy North",
					long.MaxValue
				},
				{
					"Japan East",
					long.MaxValue
				},
				{
					"Japan West",
					long.MaxValue
				},
				{
					"Jio India Central",
					long.MaxValue
				},
				{
					"Jio India West",
					long.MaxValue
				},
				{
					"Korea Central",
					long.MaxValue
				},
				{
					"Korea South",
					long.MaxValue
				},
				{
					"Malaysia South",
					long.MaxValue
				},
				{
					"Mexico Central",
					long.MaxValue
				},
				{
					"New Zealand North",
					long.MaxValue
				},
				{
					"North Central US",
					long.MaxValue
				},
				{
					"North Europe",
					long.MaxValue
				},
				{
					"Norway East",
					long.MaxValue
				},
				{
					"Norway West",
					long.MaxValue
				},
				{
					"Poland Central",
					long.MaxValue
				},
				{
					"Qatar Central",
					long.MaxValue
				},
				{
					"South Africa North",
					long.MaxValue
				},
				{
					"South Africa West",
					long.MaxValue
				},
				{
					"South Central US",
					long.MaxValue
				},
				{
					"South Central US STG",
					long.MaxValue
				},
				{
					"Southeast Asia",
					long.MaxValue
				},
				{
					"South India",
					long.MaxValue
				},
				{
					"Spain Central",
					long.MaxValue
				},
				{
					"Sweden Central",
					long.MaxValue
				},
				{
					"Sweden South",
					long.MaxValue
				},
				{
					"Switzerland North",
					long.MaxValue
				},
				{
					"Switzerland West",
					long.MaxValue
				},
				{
					"Taiwan North",
					long.MaxValue
				},
				{
					"Taiwan Northwest",
					long.MaxValue
				},
				{
					"UAE Central",
					long.MaxValue
				},
				{
					"UAE North",
					long.MaxValue
				},
				{
					"UK South",
					long.MaxValue
				},
				{
					"UK West",
					long.MaxValue
				},
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{ "USSec East", 0L },
				{ "USSec West", 255L },
				{ "USSec West Central", 255L },
				{
					"West Central US",
					long.MaxValue
				},
				{
					"West Europe",
					long.MaxValue
				},
				{
					"West India",
					long.MaxValue
				},
				{
					"West US",
					long.MaxValue
				},
				{
					"West US 2",
					long.MaxValue
				},
				{
					"West US 3",
					long.MaxValue
				},
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"USSec West",
			new Dictionary<string, long>
			{
				{
					"Australia Central",
					long.MaxValue
				},
				{
					"Australia Central 2",
					long.MaxValue
				},
				{
					"Australia East",
					long.MaxValue
				},
				{
					"Australia Southeast",
					long.MaxValue
				},
				{
					"Austria East",
					long.MaxValue
				},
				{
					"Brazil South",
					long.MaxValue
				},
				{
					"Brazil Southeast",
					long.MaxValue
				},
				{
					"Canada Central",
					long.MaxValue
				},
				{
					"Canada East",
					long.MaxValue
				},
				{
					"Central India",
					long.MaxValue
				},
				{
					"Central US",
					long.MaxValue
				},
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{
					"East Asia",
					long.MaxValue
				},
				{
					"East US",
					long.MaxValue
				},
				{
					"East US 2",
					long.MaxValue
				},
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{
					"France Central",
					long.MaxValue
				},
				{
					"France South",
					long.MaxValue
				},
				{
					"Germany North",
					long.MaxValue
				},
				{
					"Germany West Central",
					long.MaxValue
				},
				{
					"Indonesia Central",
					long.MaxValue
				},
				{
					"Israel Central",
					long.MaxValue
				},
				{
					"Italy North",
					long.MaxValue
				},
				{
					"Japan East",
					long.MaxValue
				},
				{
					"Japan West",
					long.MaxValue
				},
				{
					"Jio India Central",
					long.MaxValue
				},
				{
					"Jio India West",
					long.MaxValue
				},
				{
					"Korea Central",
					long.MaxValue
				},
				{
					"Korea South",
					long.MaxValue
				},
				{
					"Malaysia South",
					long.MaxValue
				},
				{
					"Mexico Central",
					long.MaxValue
				},
				{
					"New Zealand North",
					long.MaxValue
				},
				{
					"North Central US",
					long.MaxValue
				},
				{
					"North Europe",
					long.MaxValue
				},
				{
					"Norway East",
					long.MaxValue
				},
				{
					"Norway West",
					long.MaxValue
				},
				{
					"Poland Central",
					long.MaxValue
				},
				{
					"Qatar Central",
					long.MaxValue
				},
				{
					"South Africa North",
					long.MaxValue
				},
				{
					"South Africa West",
					long.MaxValue
				},
				{
					"South Central US",
					long.MaxValue
				},
				{
					"South Central US STG",
					long.MaxValue
				},
				{
					"Southeast Asia",
					long.MaxValue
				},
				{
					"South India",
					long.MaxValue
				},
				{
					"Spain Central",
					long.MaxValue
				},
				{
					"Sweden Central",
					long.MaxValue
				},
				{
					"Sweden South",
					long.MaxValue
				},
				{
					"Switzerland North",
					long.MaxValue
				},
				{
					"Switzerland West",
					long.MaxValue
				},
				{
					"Taiwan North",
					long.MaxValue
				},
				{
					"Taiwan Northwest",
					long.MaxValue
				},
				{
					"UAE Central",
					long.MaxValue
				},
				{
					"UAE North",
					long.MaxValue
				},
				{
					"UK South",
					long.MaxValue
				},
				{
					"UK West",
					long.MaxValue
				},
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{ "USSec East", 255L },
				{ "USSec West", 0L },
				{ "USSec West Central", 100L },
				{
					"West Central US",
					long.MaxValue
				},
				{
					"West Europe",
					long.MaxValue
				},
				{
					"West India",
					long.MaxValue
				},
				{
					"West US",
					long.MaxValue
				},
				{
					"West US 2",
					long.MaxValue
				},
				{
					"West US 3",
					long.MaxValue
				},
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"USSec West Central",
			new Dictionary<string, long>
			{
				{
					"Australia Central",
					long.MaxValue
				},
				{
					"Australia Central 2",
					long.MaxValue
				},
				{
					"Australia East",
					long.MaxValue
				},
				{
					"Australia Southeast",
					long.MaxValue
				},
				{
					"Austria East",
					long.MaxValue
				},
				{
					"Brazil South",
					long.MaxValue
				},
				{
					"Brazil Southeast",
					long.MaxValue
				},
				{
					"Canada Central",
					long.MaxValue
				},
				{
					"Canada East",
					long.MaxValue
				},
				{
					"Central India",
					long.MaxValue
				},
				{
					"Central US",
					long.MaxValue
				},
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{
					"East Asia",
					long.MaxValue
				},
				{
					"East US",
					long.MaxValue
				},
				{
					"East US 2",
					long.MaxValue
				},
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{
					"France Central",
					long.MaxValue
				},
				{
					"France South",
					long.MaxValue
				},
				{
					"Germany North",
					long.MaxValue
				},
				{
					"Germany West Central",
					long.MaxValue
				},
				{
					"Indonesia Central",
					long.MaxValue
				},
				{
					"Israel Central",
					long.MaxValue
				},
				{
					"Italy North",
					long.MaxValue
				},
				{
					"Japan East",
					long.MaxValue
				},
				{
					"Japan West",
					long.MaxValue
				},
				{
					"Jio India Central",
					long.MaxValue
				},
				{
					"Jio India West",
					long.MaxValue
				},
				{
					"Korea Central",
					long.MaxValue
				},
				{
					"Korea South",
					long.MaxValue
				},
				{
					"Malaysia South",
					long.MaxValue
				},
				{
					"Mexico Central",
					long.MaxValue
				},
				{
					"New Zealand North",
					long.MaxValue
				},
				{
					"North Central US",
					long.MaxValue
				},
				{
					"North Europe",
					long.MaxValue
				},
				{
					"Norway East",
					long.MaxValue
				},
				{
					"Norway West",
					long.MaxValue
				},
				{
					"Poland Central",
					long.MaxValue
				},
				{
					"Qatar Central",
					long.MaxValue
				},
				{
					"South Africa North",
					long.MaxValue
				},
				{
					"South Africa West",
					long.MaxValue
				},
				{
					"South Central US",
					long.MaxValue
				},
				{
					"South Central US STG",
					long.MaxValue
				},
				{
					"Southeast Asia",
					long.MaxValue
				},
				{
					"South India",
					long.MaxValue
				},
				{
					"Spain Central",
					long.MaxValue
				},
				{
					"Sweden Central",
					long.MaxValue
				},
				{
					"Sweden South",
					long.MaxValue
				},
				{
					"Switzerland North",
					long.MaxValue
				},
				{
					"Switzerland West",
					long.MaxValue
				},
				{
					"Taiwan North",
					long.MaxValue
				},
				{
					"Taiwan Northwest",
					long.MaxValue
				},
				{
					"UAE Central",
					long.MaxValue
				},
				{
					"UAE North",
					long.MaxValue
				},
				{
					"UK South",
					long.MaxValue
				},
				{
					"UK West",
					long.MaxValue
				},
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{ "USSec East", 255L },
				{ "USSec West", 100L },
				{ "USSec West Central", 0L },
				{
					"West Central US",
					long.MaxValue
				},
				{
					"West Europe",
					long.MaxValue
				},
				{
					"West India",
					long.MaxValue
				},
				{
					"West US",
					long.MaxValue
				},
				{
					"West US 2",
					long.MaxValue
				},
				{
					"West US 3",
					long.MaxValue
				},
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"West Central US",
			new Dictionary<string, long>
			{
				{ "Australia Central", 170L },
				{ "Australia Central 2", 170L },
				{ "Australia East", 166L },
				{ "Australia Southeast", 176L },
				{ "Austria East", 134L },
				{ "Brazil South", 160L },
				{ "Brazil Southeast", 160L },
				{ "Canada Central", 36L },
				{ "Canada East", 44L },
				{ "Central India", 236L },
				{ "Central US", 15L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 162L },
				{ "East US", 38L },
				{ "East US 2", 47L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 119L },
				{ "France South", 126L },
				{ "Germany North", 132L },
				{ "Germany West Central", 126L },
				{ "Indonesia Central", 180L },
				{ "Israel Central", 230L },
				{ "Italy North", 134L },
				{ "Japan East", 126L },
				{ "Japan West", 128L },
				{ "Jio India Central", 236L },
				{ "Jio India West", 236L },
				{ "Korea Central", 143L },
				{ "Korea South", 148L },
				{ "Malaysia South", 180L },
				{ "Mexico Central", 22L },
				{ "New Zealand North", 170L },
				{ "North Central US", 22L },
				{ "North Europe", 110L },
				{ "Norway East", 142L },
				{ "Norway West", 136L },
				{ "Poland Central", 120L },
				{ "Qatar Central", 230L },
				{ "South Africa North", 274L },
				{ "South Africa West", 258L },
				{ "South Central US", 22L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 180L },
				{ "South India", 212L },
				{ "Spain Central", 119L },
				{ "Sweden Central", 142L },
				{ "Sweden South", 142L },
				{ "Switzerland North", 134L },
				{ "Switzerland West", 126L },
				{ "Taiwan North", 162L },
				{ "Taiwan Northwest", 162L },
				{ "UAE Central", 230L },
				{ "UAE North", 234L },
				{ "UK South", 110L },
				{ "UK West", 112L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 0L },
				{ "West Europe", 120L },
				{ "West India", 240L },
				{ "West US", 30L },
				{ "West US 2", 22L },
				{ "West US 3", 22L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"West Europe",
			new Dictionary<string, long>
			{
				{ "Australia Central", 252L },
				{ "Australia Central 2", 252L },
				{ "Australia East", 248L },
				{ "Australia Southeast", 244L },
				{ "Austria East", 12L },
				{ "Brazil South", 188L },
				{ "Brazil Southeast", 188L },
				{ "Canada Central", 94L },
				{ "Canada East", 103L },
				{ "Central India", 126L },
				{ "Central US", 102L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 193L },
				{ "East US", 81L },
				{ "East US 2", 86L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 10L },
				{ "France South", 20L },
				{ "Germany North", 10L },
				{ "Germany West Central", 8L },
				{ "Indonesia Central", 160L },
				{ "Israel Central", 118L },
				{ "Italy North", 12L },
				{ "Japan East", 234L },
				{ "Japan West", 228L },
				{ "Jio India Central", 126L },
				{ "Jio India West", 126L },
				{ "Korea Central", 222L },
				{ "Korea South", 216L },
				{ "Malaysia South", 160L },
				{ "Mexico Central", 142L },
				{ "New Zealand North", 252L },
				{ "North Central US", 102L },
				{ "North Europe", 22L },
				{ "Norway East", 22L },
				{ "Norway West", 15L },
				{ "Poland Central", 100L },
				{ "Qatar Central", 118L },
				{ "South Africa North", 170L },
				{ "South Africa West", 152L },
				{ "South Central US", 112L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 160L },
				{ "South India", 136L },
				{ "Spain Central", 10L },
				{ "Sweden Central", 22L },
				{ "Sweden South", 22L },
				{ "Switzerland North", 12L },
				{ "Switzerland West", 16L },
				{ "Taiwan North", 193L },
				{ "Taiwan Northwest", 193L },
				{ "UAE Central", 118L },
				{ "UAE North", 120L },
				{ "UK South", 10L },
				{ "UK West", 12L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 120L },
				{ "West Europe", 0L },
				{ "West India", 128L },
				{ "West US", 144L },
				{ "West US 2", 142L },
				{ "West US 3", 142L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"West India",
			new Dictionary<string, long>
			{
				{ "Australia Central", 142L },
				{ "Australia Central 2", 142L },
				{ "Australia East", 138L },
				{ "Australia Southeast", 134L },
				{ "Austria East", 116L },
				{ "Brazil South", 304L },
				{ "Brazil Southeast", 304L },
				{ "Canada Central", 210L },
				{ "Canada East", 220L },
				{ "Central India", 4L },
				{ "Central US", 224L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 84L },
				{ "East US", 198L },
				{ "East US 2", 200L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 118L },
				{ "France South", 106L },
				{ "Germany North", 130L },
				{ "Germany West Central", 122L },
				{ "Indonesia Central", 50L },
				{ "Israel Central", 28L },
				{ "Italy North", 116L },
				{ "Japan East", 118L },
				{ "Japan West", 122L },
				{ "Jio India Central", 4L },
				{ "Jio India West", 4L },
				{ "Korea Central", 112L },
				{ "Korea South", 106L },
				{ "Malaysia South", 50L },
				{ "Mexico Central", 210L },
				{ "New Zealand North", 142L },
				{ "North Central US", 218L },
				{ "North Europe", 139L },
				{ "Norway East", 146L },
				{ "Norway West", 140L },
				{ "Poland Central", 128L },
				{ "Qatar Central", 28L },
				{ "South Africa North", 266L },
				{ "South Africa West", 270L },
				{ "South Central US", 226L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 50L },
				{ "South India", 20L },
				{ "Spain Central", 118L },
				{ "Sweden Central", 146L },
				{ "Sweden South", 146L },
				{ "Switzerland North", 116L },
				{ "Switzerland West", 112L },
				{ "Taiwan North", 84L },
				{ "Taiwan Northwest", 84L },
				{ "UAE Central", 28L },
				{ "UAE North", 28L },
				{ "UK South", 126L },
				{ "UK West", 128L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 240L },
				{ "West Europe", 128L },
				{ "West India", 0L },
				{ "West US", 217L },
				{ "West US 2", 210L },
				{ "West US 3", 210L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"West US",
			new Dictionary<string, long>
			{
				{ "Australia Central", 142L },
				{ "Australia Central 2", 142L },
				{ "Australia East", 138L },
				{ "Australia Southeast", 148L },
				{ "Austria East", 150L },
				{ "Brazil South", 172L },
				{ "Brazil Southeast", 172L },
				{ "Canada Central", 64L },
				{ "Canada East", 72L },
				{ "Central India", 218L },
				{ "Central US", 44L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 148L },
				{ "East US", 64L },
				{ "East US 2", 58L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 138L },
				{ "France South", 148L },
				{ "Germany North", 153L },
				{ "Germany West Central", 148L },
				{ "Indonesia Central", 168L },
				{ "Israel Central", 244L },
				{ "Italy North", 150L },
				{ "Japan East", 106L },
				{ "Japan West", 106L },
				{ "Jio India Central", 218L },
				{ "Jio India West", 218L },
				{ "Korea Central", 130L },
				{ "Korea South", 134L },
				{ "Malaysia South", 168L },
				{ "Mexico Central", 22L },
				{ "New Zealand North", 142L },
				{ "North Central US", 50L },
				{ "North Europe", 133L },
				{ "Norway East", 163L },
				{ "Norway West", 156L },
				{ "Poland Central", 144L },
				{ "Qatar Central", 244L },
				{ "South Africa North", 298L },
				{ "South Africa West", 280L },
				{ "South Central US", 34L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 168L },
				{ "South India", 200L },
				{ "Spain Central", 138L },
				{ "Sweden Central", 163L },
				{ "Sweden South", 163L },
				{ "Switzerland North", 150L },
				{ "Switzerland West", 147L },
				{ "Taiwan North", 148L },
				{ "Taiwan Northwest", 148L },
				{ "UAE Central", 244L },
				{ "UAE North", 244L },
				{ "UK South", 136L },
				{ "UK West", 138L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 30L },
				{ "West Europe", 144L },
				{ "West India", 217L },
				{ "West US", 0L },
				{ "West US 2", 22L },
				{ "West US 3", 22L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"West US 2",
			new Dictionary<string, long>
			{
				{ "Australia Central", 162L },
				{ "Australia Central 2", 162L },
				{ "Australia East", 158L },
				{ "Australia Southeast", 168L },
				{ "Austria East", 154L },
				{ "Brazil South", 182L },
				{ "Brazil Southeast", 182L },
				{ "Canada Central", 57L },
				{ "Canada East", 66L },
				{ "Central India", 212L },
				{ "Central US", 36L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 140L },
				{ "East US", 58L },
				{ "East US 2", 68L },
				{
					"East US SLV",
					long.MaxValue
				},
				{ "East US STG", 58L },
				{ "France Central", 140L },
				{ "France South", 146L },
				{ "Germany North", 154L },
				{ "Germany West Central", 148L },
				{ "Indonesia Central", 160L },
				{ "Israel Central", 236L },
				{ "Italy North", 154L },
				{ "Japan East", 112L },
				{ "Japan West", 112L },
				{ "Jio India Central", 212L },
				{ "Jio India West", 212L },
				{ "Korea Central", 122L },
				{ "Korea South", 126L },
				{ "Malaysia South", 160L },
				{ "Mexico Central", 100L },
				{ "New Zealand North", 162L },
				{ "North Central US", 44L },
				{ "North Europe", 136L },
				{ "Norway East", 164L },
				{ "Norway West", 156L },
				{ "Poland Central", 142L },
				{ "Qatar Central", 236L },
				{ "South Africa North", 296L },
				{ "South Africa West", 280L },
				{ "South Central US", 44L },
				{ "South Central US STG", 44L },
				{ "Southeast Asia", 160L },
				{ "South India", 192L },
				{ "Spain Central", 140L },
				{ "Sweden Central", 164L },
				{ "Sweden South", 164L },
				{ "Switzerland North", 154L },
				{ "Switzerland West", 146L },
				{ "Taiwan North", 140L },
				{ "Taiwan Northwest", 140L },
				{ "UAE Central", 236L },
				{ "UAE North", 236L },
				{ "UK South", 130L },
				{ "UK West", 132L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 22L },
				{ "West Europe", 142L },
				{ "West India", 210L },
				{ "West US", 22L },
				{ "West US 2", 0L },
				{ "West US 3", 32L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"West US 3",
			new Dictionary<string, long>
			{
				{ "Australia Central", 162L },
				{ "Australia Central 2", 162L },
				{ "Australia East", 158L },
				{ "Australia Southeast", 168L },
				{ "Austria East", 154L },
				{ "Brazil South", 182L },
				{ "Brazil Southeast", 182L },
				{ "Canada Central", 57L },
				{ "Canada East", 66L },
				{ "Central India", 212L },
				{ "Central US", 36L },
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{ "East Asia", 140L },
				{ "East US", 58L },
				{ "East US 2", 68L },
				{
					"East US SLV",
					long.MaxValue
				},
				{
					"East US STG",
					long.MaxValue
				},
				{ "France Central", 140L },
				{ "France South", 146L },
				{ "Germany North", 154L },
				{ "Germany West Central", 148L },
				{ "Indonesia Central", 160L },
				{ "Israel Central", 236L },
				{ "Italy North", 154L },
				{ "Japan East", 112L },
				{ "Japan West", 112L },
				{ "Jio India Central", 212L },
				{ "Jio India West", 212L },
				{ "Korea Central", 122L },
				{ "Korea South", 126L },
				{ "Malaysia South", 160L },
				{ "Mexico Central", 100L },
				{ "New Zealand North", 162L },
				{ "North Central US", 44L },
				{ "North Europe", 136L },
				{ "Norway East", 164L },
				{ "Norway West", 156L },
				{ "Poland Central", 142L },
				{ "Qatar Central", 236L },
				{ "South Africa North", 296L },
				{ "South Africa West", 280L },
				{ "South Central US", 44L },
				{
					"South Central US STG",
					long.MaxValue
				},
				{ "Southeast Asia", 160L },
				{ "South India", 192L },
				{ "Spain Central", 140L },
				{ "Sweden Central", 164L },
				{ "Sweden South", 164L },
				{ "Switzerland North", 154L },
				{ "Switzerland West", 146L },
				{ "Taiwan North", 140L },
				{ "Taiwan Northwest", 140L },
				{ "UAE Central", 236L },
				{ "UAE North", 236L },
				{ "UK South", 130L },
				{ "UK West", 132L },
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{ "West Central US", 22L },
				{ "West Europe", 142L },
				{ "West India", 210L },
				{ "West US", 22L },
				{ "West US 2", 32L },
				{ "West US 3", 0L },
				{
					"Central US EUAP",
					long.MaxValue
				},
				{
					"East US 2 EUAP",
					long.MaxValue
				}
			}
		},
		{
			"Central US EUAP",
			new Dictionary<string, long>
			{
				{
					"Australia Central",
					long.MaxValue
				},
				{
					"Australia Central 2",
					long.MaxValue
				},
				{
					"Australia East",
					long.MaxValue
				},
				{
					"Australia Southeast",
					long.MaxValue
				},
				{
					"Austria East",
					long.MaxValue
				},
				{
					"Brazil South",
					long.MaxValue
				},
				{
					"Brazil Southeast",
					long.MaxValue
				},
				{
					"Canada Central",
					long.MaxValue
				},
				{
					"Canada East",
					long.MaxValue
				},
				{
					"Central India",
					long.MaxValue
				},
				{
					"Central US",
					long.MaxValue
				},
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{
					"East Asia",
					long.MaxValue
				},
				{
					"East US",
					long.MaxValue
				},
				{
					"East US 2",
					long.MaxValue
				},
				{ "East US SLV", 255L },
				{
					"East US STG",
					long.MaxValue
				},
				{
					"France Central",
					long.MaxValue
				},
				{
					"France South",
					long.MaxValue
				},
				{
					"Germany North",
					long.MaxValue
				},
				{
					"Germany West Central",
					long.MaxValue
				},
				{
					"Indonesia Central",
					long.MaxValue
				},
				{
					"Israel Central",
					long.MaxValue
				},
				{
					"Italy North",
					long.MaxValue
				},
				{
					"Japan East",
					long.MaxValue
				},
				{
					"Japan West",
					long.MaxValue
				},
				{
					"Jio India Central",
					long.MaxValue
				},
				{
					"Jio India West",
					long.MaxValue
				},
				{
					"Korea Central",
					long.MaxValue
				},
				{
					"Korea South",
					long.MaxValue
				},
				{
					"Malaysia South",
					long.MaxValue
				},
				{
					"Mexico Central",
					long.MaxValue
				},
				{
					"New Zealand North",
					long.MaxValue
				},
				{
					"North Central US",
					long.MaxValue
				},
				{
					"North Europe",
					long.MaxValue
				},
				{
					"Norway East",
					long.MaxValue
				},
				{
					"Norway West",
					long.MaxValue
				},
				{
					"Poland Central",
					long.MaxValue
				},
				{
					"Qatar Central",
					long.MaxValue
				},
				{
					"South Africa North",
					long.MaxValue
				},
				{
					"South Africa West",
					long.MaxValue
				},
				{
					"South Central US",
					long.MaxValue
				},
				{
					"South Central US STG",
					long.MaxValue
				},
				{
					"Southeast Asia",
					long.MaxValue
				},
				{
					"South India",
					long.MaxValue
				},
				{
					"Spain Central",
					long.MaxValue
				},
				{
					"Sweden Central",
					long.MaxValue
				},
				{
					"Sweden South",
					long.MaxValue
				},
				{
					"Switzerland North",
					long.MaxValue
				},
				{
					"Switzerland West",
					long.MaxValue
				},
				{
					"Taiwan North",
					long.MaxValue
				},
				{
					"Taiwan Northwest",
					long.MaxValue
				},
				{
					"UAE Central",
					long.MaxValue
				},
				{
					"UAE North",
					long.MaxValue
				},
				{
					"UK South",
					long.MaxValue
				},
				{
					"UK West",
					long.MaxValue
				},
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{
					"West Central US",
					long.MaxValue
				},
				{
					"West Europe",
					long.MaxValue
				},
				{
					"West India",
					long.MaxValue
				},
				{
					"West US",
					long.MaxValue
				},
				{
					"West US 2",
					long.MaxValue
				},
				{
					"West US 3",
					long.MaxValue
				},
				{ "Central US EUAP", 0L },
				{ "East US 2 EUAP", 40L }
			}
		},
		{
			"East US 2 EUAP",
			new Dictionary<string, long>
			{
				{
					"Australia Central",
					long.MaxValue
				},
				{
					"Australia Central 2",
					long.MaxValue
				},
				{
					"Australia East",
					long.MaxValue
				},
				{
					"Australia Southeast",
					long.MaxValue
				},
				{
					"Austria East",
					long.MaxValue
				},
				{
					"Brazil South",
					long.MaxValue
				},
				{
					"Brazil Southeast",
					long.MaxValue
				},
				{
					"Canada Central",
					long.MaxValue
				},
				{
					"Canada East",
					long.MaxValue
				},
				{
					"Central India",
					long.MaxValue
				},
				{
					"Central US",
					long.MaxValue
				},
				{
					"China East",
					long.MaxValue
				},
				{
					"China East 2",
					long.MaxValue
				},
				{
					"China East 3",
					long.MaxValue
				},
				{
					"China North",
					long.MaxValue
				},
				{
					"China North 2",
					long.MaxValue
				},
				{
					"China North 3",
					long.MaxValue
				},
				{
					"East Asia",
					long.MaxValue
				},
				{
					"East US",
					long.MaxValue
				},
				{
					"East US 2",
					long.MaxValue
				},
				{ "East US SLV", 255L },
				{
					"East US STG",
					long.MaxValue
				},
				{
					"France Central",
					long.MaxValue
				},
				{
					"France South",
					long.MaxValue
				},
				{
					"Germany North",
					long.MaxValue
				},
				{
					"Germany West Central",
					long.MaxValue
				},
				{
					"Indonesia Central",
					long.MaxValue
				},
				{
					"Israel Central",
					long.MaxValue
				},
				{
					"Italy North",
					long.MaxValue
				},
				{
					"Japan East",
					long.MaxValue
				},
				{
					"Japan West",
					long.MaxValue
				},
				{
					"Jio India Central",
					long.MaxValue
				},
				{
					"Jio India West",
					long.MaxValue
				},
				{
					"Korea Central",
					long.MaxValue
				},
				{
					"Korea South",
					long.MaxValue
				},
				{
					"Malaysia South",
					long.MaxValue
				},
				{
					"Mexico Central",
					long.MaxValue
				},
				{
					"New Zealand North",
					long.MaxValue
				},
				{
					"North Central US",
					long.MaxValue
				},
				{
					"North Europe",
					long.MaxValue
				},
				{
					"Norway East",
					long.MaxValue
				},
				{
					"Norway West",
					long.MaxValue
				},
				{
					"Poland Central",
					long.MaxValue
				},
				{
					"Qatar Central",
					long.MaxValue
				},
				{
					"South Africa North",
					long.MaxValue
				},
				{
					"South Africa West",
					long.MaxValue
				},
				{
					"South Central US",
					long.MaxValue
				},
				{
					"South Central US STG",
					long.MaxValue
				},
				{
					"Southeast Asia",
					long.MaxValue
				},
				{
					"South India",
					long.MaxValue
				},
				{
					"Spain Central",
					long.MaxValue
				},
				{
					"Sweden Central",
					long.MaxValue
				},
				{
					"Sweden South",
					long.MaxValue
				},
				{
					"Switzerland North",
					long.MaxValue
				},
				{
					"Switzerland West",
					long.MaxValue
				},
				{
					"Taiwan North",
					long.MaxValue
				},
				{
					"Taiwan Northwest",
					long.MaxValue
				},
				{
					"UAE Central",
					long.MaxValue
				},
				{
					"UAE North",
					long.MaxValue
				},
				{
					"UK South",
					long.MaxValue
				},
				{
					"UK West",
					long.MaxValue
				},
				{
					"USDoD Central",
					long.MaxValue
				},
				{
					"USDoD East",
					long.MaxValue
				},
				{
					"USGov Arizona",
					long.MaxValue
				},
				{
					"USGov Texas",
					long.MaxValue
				},
				{
					"USGov Virginia",
					long.MaxValue
				},
				{
					"USNat East",
					long.MaxValue
				},
				{
					"USNat West",
					long.MaxValue
				},
				{
					"USSec East",
					long.MaxValue
				},
				{
					"USSec West",
					long.MaxValue
				},
				{
					"USSec West Central",
					long.MaxValue
				},
				{
					"West Central US",
					long.MaxValue
				},
				{
					"West Europe",
					long.MaxValue
				},
				{
					"West India",
					long.MaxValue
				},
				{
					"West US",
					long.MaxValue
				},
				{
					"West US 2",
					long.MaxValue
				},
				{
					"West US 3",
					long.MaxValue
				},
				{ "Central US EUAP", 40L },
				{ "East US 2 EUAP", 0L }
			}
		}
	};

	public static List<string> GetRegionsForLinkType(GeoLinkTypes geoLinkType, List<string> existingRegions)
	{
		foreach (string existingRegion in existingRegions)
		{
			if (string.IsNullOrEmpty(existingRegion) || !SourceRegionToTargetRegionsRTTInMs.ContainsKey(existingRegion))
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "existingRegion {0} is not a valid region.", existingRegion));
			}
		}
		List<List<string>> list = new List<List<string>>();
		foreach (string existingRegion2 in existingRegions)
		{
			list.Add(GetRegionsForLinkType(geoLinkType, existingRegion2));
		}
		if (list.Count < 1)
		{
			return new List<string>();
		}
		IEnumerable<string> enumerable = list[0].AsEnumerable();
		for (int i = 1; i < list.Count; i++)
		{
			enumerable = enumerable.Intersect(list[i]);
		}
		if (existingRegions.Except(enumerable).Any())
		{
			return new List<string>();
		}
		return enumerable.ToList();
	}

	public static List<string> GetRegionsForLinkType(GeoLinkTypes geoLinkType, string sourceRegion)
	{
		if (string.IsNullOrEmpty(sourceRegion) || !SourceRegionToTargetRegionsRTTInMs.ContainsKey(sourceRegion))
		{
			throw new ArgumentException("sourceRegion is not a valid region.");
		}
		List<string> list = new List<string>();
		long linkTypeThresholdInMs = GetLinkTypeThresholdInMs(geoLinkType);
		Dictionary<string, long> dictionary = SourceRegionToTargetRegionsRTTInMs[sourceRegion];
		foreach (string key in dictionary.Keys)
		{
			if (dictionary[key] <= linkTypeThresholdInMs)
			{
				list.Add(key);
			}
		}
		if (!list.Contains(sourceRegion))
		{
			return new List<string>();
		}
		return list;
	}

	public static List<string> GeneratePreferredRegionList(string sourceRegion)
	{
		if (string.IsNullOrEmpty(sourceRegion) || !SourceRegionToTargetRegionsRTTInMs.ContainsKey(sourceRegion))
		{
			throw new ArgumentException("sourceRegion is not a valid region.");
		}
		List<KeyValuePair<string, long>> list = SourceRegionToTargetRegionsRTTInMs[sourceRegion].ToList();
		list.Sort((KeyValuePair<string, long> x, KeyValuePair<string, long> y) => x.Value.CompareTo(y.Value));
		List<string> list2 = new List<string>();
		foreach (KeyValuePair<string, long> item in list)
		{
			list2.Add(item.Key);
		}
		return list2;
	}

	private static long GetLinkTypeThresholdInMs(GeoLinkTypes geoLinkType)
	{
		return geoLinkType switch
		{
			GeoLinkTypes.Strong => 100L, 
			GeoLinkTypes.Medium => 200L, 
			GeoLinkTypes.Weak => long.MaxValue, 
			_ => throw new ArgumentException("GeoLinkType provided is invalid."), 
		};
	}
}
