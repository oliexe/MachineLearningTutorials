## MAD 3 - Cvièení 2 (2/10/17)
## Ondøej Øeháèek - REH0063

install.packages("igraph")
install.packages("data.table")
library(igraph)
library(data.table)

## Load input file 
input <- fread('ht09_contact_list.tsv', sep = '\t', select = c(2:3), header = TRUE)


## Graph the file
g <- graph.data.frame(input, directed = FALSE)
V(g)$vertex_degree <- degree(g)
plot(g, edge.width = V(input)$weight, vertex.size = 10)