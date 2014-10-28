

/****** Object:  Table [dbo].[B_CheckBatchRunning]    Script Date: 10/28/2014 11:16:52 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[B_CheckBatchRunning](
	[BatchName] [nvarchar](200) NOT NULL,
	[RunningFlag] [nvarchar](50) NULL,
	[UpdatedDate] [datetime] NULL,
	[NoOfRuns] [int] NULL,
 CONSTRAINT [PK_B_CheckBatchRunning] PRIMARY KEY CLUSTERED 
(
	[BatchName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


