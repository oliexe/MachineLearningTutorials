library("dbscan")
data <- read.csv(file = "c:/vgsales.csv")[ ,c('EU_Sales', 'NA_Sales')]

x <- as.matrix(data[, 1:2])

## DBSCAN
db <- dbscan(x, eps = 1.0, minPts = 2)
db

pairs(x, col = db$cluster + 1L)
