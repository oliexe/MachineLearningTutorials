## MAD 3 - Cvièení 1
## Ondøej Øeháèek - REH0063

## Instalovat igrapgh package
install.packages("igraph")

## igraph
library(igraph)

## 1a) Random graph
rand <- sample_gnm(n = 100, m = 250)
plot(rand, vertex.size = 6, vertex.label = NA)

## 1b) Barabási–Albert model
bara <- make_tree(40, children = 3, mode = "undirected")
plot(bara, vertex.size = 10, vertex.label = NA)

## 2) Matice sousednosti
as_adjacency_matrix(rand)
as_adjacency_matrix(bara)

## 2c) Prùmìrná støední hodnota nejkratší cesty
mean_distance(rand)
mean_distance(bara)

## 2d) Stupnì jednotlivých vrcholù + max stupeò
degree(rand)
max(degree(rand))
degree(bara)
max(degree(bara))