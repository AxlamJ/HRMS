SELECT
  e.Id,
  e.FirstName,
  e.HiringDate,
  -- merge into one shorthand column, casting each part
  CAST(ca.Years  AS varchar(3)) + 'y '
  + CAST(ca.Months AS varchar(3)) + 'm '
  + CAST(ca.Days   AS varchar(3)) + 'd'      AS DurationOnProbation
FROM
  Employee AS e
  CROSS APPLY
  (
    SELECT
      -- full years
      DATEDIFF(YEAR, e.HiringDate, GETDATE())
        - CASE
            WHEN DATEADD(
                   YEAR,
                   DATEDIFF(YEAR, e.HiringDate, GETDATE()),
                   e.HiringDate
                 ) > GETDATE()
             THEN 1 ELSE 0
          END
        AS Years,

      -- remaining months
      DATEDIFF(
        MONTH,
        DATEADD(
          YEAR,
          DATEDIFF(YEAR, e.HiringDate, GETDATE())
            - CASE
                WHEN DATEADD(
                       YEAR,
                       DATEDIFF(YEAR, e.HiringDate, GETDATE()),
                       e.HiringDate
                     ) > GETDATE()
                 THEN 1 ELSE 0
              END,
          e.HiringDate
        ),
        GETDATE()
      )
      - CASE WHEN DAY(GETDATE()) < DAY(e.HiringDate) THEN 1 ELSE 0 END
        AS Months,

      -- remaining days
      DATEDIFF(
        DAY,
        DATEADD(
          MONTH,
          DATEDIFF(
            MONTH,
            DATEADD(
              YEAR,
              DATEDIFF(YEAR, e.HiringDate, GETDATE())
                - CASE
                    WHEN DATEADD(
                           YEAR,
                           DATEDIFF(YEAR, e.HiringDate, GETDATE()),
                           e.HiringDate
                         ) > GETDATE()
                     THEN 1 ELSE 0
                  END,
              e.HiringDate
            ),
            GETDATE()
          )
            - CASE WHEN DAY(GETDATE()) < DAY(e.HiringDate) THEN 1 ELSE 0 END,
          DATEADD(
            YEAR,
            DATEDIFF(YEAR, e.HiringDate, GETDATE())
              - CASE
                  WHEN DATEADD(
                         YEAR,
                         DATEDIFF(YEAR, e.HiringDate, GETDATE()),
                         e.HiringDate
                       ) > GETDATE()
                   THEN 1 ELSE 0
                END,
            e.HiringDate
          )
        ),
        GETDATE()
      ) AS Days
  ) AS ca
WHERE
  e.HiringDate IS NOT NULL;
