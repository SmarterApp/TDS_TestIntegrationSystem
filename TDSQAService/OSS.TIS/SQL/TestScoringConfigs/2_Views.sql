/****** Object:  View [dbo].[_RawParameter]    Script Date: 01/16/2015 11:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[_RawParameter]
AS

select _fk_TestScoreFeature, ParameterName, ParameterPosition,
	case IndexType
		when 'string' then '''' + [Index] + ''''
		else [Index]
	end as [Index],
	case Type
		when 'string' then '''' + [Value] + ''''
		else [Value]
	end as [Value]
from
	ComputationRuleParameterValue V1
	join ComputationRuleParameters P1 on V1._fk_Parameter = P1._Key
GO
/****** Object:  View [dbo].[_ComputationLocations]    Script Date: 01/16/2015 11:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[_ComputationLocations]
AS
	select _fk_TestScoreFeature, left(A.locs,len(A.locs)-1) as locations
	from
		(
			select distinct _fk_TestScoreFeature, 
				(
					select Location + ', ' as [text()]
					from
						Feature_ComputationLocation fl1
					where
						fl1._fk_TestScoreFeature = fl._fk_TestScoreFeature
					order by
						Location
					for xml path('')
				) as locs
			from 
				Feature_ComputationLocation fl
		) A
GO
/****** Object:  View [dbo].[v__RawParameters]    Script Date: 01/16/2015 11:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[v__RawParameters]
AS

select _fk_TestScoreFeature, ParameterName, ParameterPosition,
	case IndexType
		when 'string' then '''' + [Index] + ''''
		else [Index]
	end as [Index],
	case Type
		when 'string' then '''' + [Value] + ''''
		else [Value]
	end as [Value]
from
	ComputationRuleParameterValue V1
	join ComputationRuleParameters P1 on V1._fk_Parameter = P1._Key
GO
/****** Object:  View [dbo].[v__Parameters]    Script Date: 01/16/2015 11:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[v__Parameters]
AS

select *
from
	(
		select _fk_TestScoreFeature, ParameterName, ParameterPosition,
			'[' + replace(left(A.parVal,len(A.parVal)-1),'&gt;','>') + ']' as parameterValue
		from
			(
				select distinct _fk_TestScoreFeature, ParameterName, ParameterPosition,
					(
						select [Index] + '=>' + [Value] + ', ' as [text()]
						from
							v__RawParameters P1
						where
							P1._fk_TestScoreFeature = P._fk_TestScoreFeature and P1.ParameterName = P.ParameterName and P1.ParameterPosition = P.ParameterPosition
						for xml path('')
					) as parVal
				from 
					v__RawParameters P
				where
					[Index] != ''
			) A

		UNION ALL

		select
			_fk_TestScoreFeature, ParameterName, ParameterPosition, [Value] as parameterValue
		from 
			v__RawParameters P1
		where
			[Index] = ''
	) B
GO
/****** Object:  View [dbo].[_Parameter]    Script Date: 01/16/2015 11:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[_Parameter]
AS

select *
from
	(
		select _fk_TestScoreFeature, ParameterName, ParameterPosition,
			'[' + replace(left(A.parVal,len(A.parVal)-1),'&gt;','>') + ']' as parameterValue
		from
			(
				select distinct _fk_TestScoreFeature, ParameterName, ParameterPosition,
					(
						select [Index] + '=>' + [Value] + ', ' as [text()]
						from
							_RawParameter P1
						where
							P1._fk_TestScoreFeature = P._fk_TestScoreFeature and P1.ParameterName = P.ParameterName and P1.ParameterPosition = P.ParameterPosition
						for xml path('')
					) as parVal
				from 
					_RawParameter P
				where
					[Index] != ''
			) A

		UNION ALL

		select
			_fk_TestScoreFeature, ParameterName, ParameterPosition, [Value] as parameterValue
		from 
			_RawParameter P1
		where
			[Index] = ''
	) B
GO
/****** Object:  View [dbo].[_Parameters]    Script Date: 01/16/2015 11:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[_Parameters]
AS
	select _fk_TestScoreFeature, replace(left(A.pars,len(A.pars)-1),'&gt;','>') as parameters
	from
		(
			select distinct _fk_TestScoreFeature, 
				(
					select ParameterName + '=' + parameterValue + ', ' as [text()]
					from
						_Parameter P1
					where
						P1._fk_TestScoreFeature = P._fk_TestScoreFeature
					order by
						ParameterPosition
					for xml path('')
				) as pars
			from 
				_Parameter P
		) A
GO
/****** Object:  View [dbo].[v_TestScoreSummary]    Script Date: 01/16/2015 11:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[v_TestScoreSummary]
AS
	select clientname, TestID, MeasureOf, MeasureLabel, ComputationOrder, locations as ComputationLocations,
		ComputationRule + '(' + isnull(parameters,'') + ')' as [Function]
	from
		TestScoreFeature F
		left join _Parameters P on F._Key = P._fk_TestScoreFeature
		left join _ComputationLocations C on F._Key = C._fk_TestScoreFeature
GO
