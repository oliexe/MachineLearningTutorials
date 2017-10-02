## MAD 3 - Cvièení 1
## Ondøej Øeháèek - REH0063

install.packages("igraph")
library(igraph)

## --------------------1--------------------

## 1a) Random graph + plot
rand <- sample_gnm(n = 200, m = 600)
plot(rand, vertex.size = 2, vertex.label = NA)

## 1b) Barabási–Albert model + plot
bara <- make_tree(1000, children = 3, mode = "undirected")
plot(bara, vertex.size = 2, vertex.label = NA)

## --------------------2--------------------

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

## --------------------3--------------------

## 3a) Cilené odebární maximálního stupnì
subgraph_maxdegree_rand_1 = delete_vertices(rand, max(degree(rand)))
subgraph_maxdegree_bara_1 = delete_vertices(bara, max(degree(bara)))

## 3a) Cilené odebární nahodného stupnì
subgraph_random_rand_1 = delete_vertices(rand, 3)
subgraph_random_bara_1 = delete_vertices(bara, 3)

