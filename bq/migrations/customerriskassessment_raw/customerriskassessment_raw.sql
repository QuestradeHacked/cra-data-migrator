CREATE TABLE IF NOT EXISTS `customerriskassessment_raw.risk_ratings`
(
  riskRatingId STRING NOT NULL,
  customerId STRING NOT NULL,
  customerFullName STRING NOT NULL,
  totalPoints INT64 NOT NULL,
  rating STRING NOT NULL,
  previousRating STRING,
  ratingTime DATETIME NOT NULL,
  factors ARRAY<STRUCT<factorName STRING NOT NULL, factorValue STRING, previousFactorValue STRING, points INT64 NOT NULL>>
)
PARTITION BY DATE(ratingTime);