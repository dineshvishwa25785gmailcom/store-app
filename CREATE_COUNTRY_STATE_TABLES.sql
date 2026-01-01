USE [store_app]
GO

-- Create Country Master Table
CREATE TABLE [dbo].[tbl_country](
	[country_code] [varchar](3) NOT NULL,
	[country_name] [nvarchar](100) NOT NULL,
	[is_active] [bit] NOT NULL DEFAULT 1,
 CONSTRAINT [PK_tbl_country] PRIMARY KEY CLUSTERED ([country_code] ASC)
)
GO

-- Create State Master Table
CREATE TABLE [dbo].[tbl_state](
	[state_code] [varchar](3) NOT NULL,
	[state_name] [nvarchar](100) NOT NULL,
	[country_code] [varchar](3) NOT NULL,
	[is_active] [bit] NOT NULL DEFAULT 1,
 CONSTRAINT [PK_tbl_state] PRIMARY KEY CLUSTERED ([state_code] ASC),
 CONSTRAINT [FK_tbl_state_country] FOREIGN KEY([country_code]) REFERENCES [dbo].[tbl_country]([country_code])
)
GO

-- Insert Countries
INSERT INTO tbl_country (country_code, country_name, is_active) VALUES
('IN', 'India', 1),
('US', 'USA', 1),
('GB', 'UK', 1);
GO

-- Insert States
INSERT INTO tbl_state (state_code, state_name, country_code, is_active) VALUES
('UP', 'Uttar Pradesh', 'IN', 1),
('MH', 'Maharashtra', 'IN', 1),
('KA', 'Karnataka', 'IN', 1),
('DL', 'Delhi', 'IN', 1),
('CA', 'California', 'US', 1),
('TX', 'Texas', 'US', 1),
('NY', 'New York', 'US', 1),
('LN', 'London', 'GB', 1),
('MC', 'Manchester', 'GB', 1);
GO
