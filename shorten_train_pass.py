from __future__ import annotations 

class Node:
    name: str
    neighbors: list[Node]

    def __init__(self, name: str):
        self.name = name
        self.neighbors = []
        pass

    def add_connection(self, node: Node):
        if node not in self.neighbors:
            self.neighbors.append(node)

    def __eq__(self, value: Node):
        return self.name == value.name

    def __hash__(self):
        return hash(self.name)
    
def train_soccer(trains: list[str]) -> list[str]:
    starting_node = construct_graph(trains)

    return [node.name for node in calculate_shortest_distance(starting_node, trains[-1])]

def construct_graph(trains: list[str]) -> Node:
    existing_nodes = {}

    previous_node: Node = None

    for train in trains:
        train_node = existing_nodes.get(train)

        if train_node is None:
            train_node = Node(train)
            existing_nodes[train] = train_node

        if previous_node is not None:
            previous_node.add_connection(train_node)
        
        previous_node = train_node

    return existing_nodes[trains[0]]

def calculate_shortest_distance(start: Node, goal: str) -> list[Node]:
    queue: list[list[Node]] = [[start]]
    visited: set[Node] = set([start])

    while len(queue) > 0:
        path: list[Node] = queue.pop(0)
        node: Node = path[-1]

        if node.name == goal:
            return path

        for neighbor in node.neighbors:
            if neighbor in visited:
                continue

            visited.add(neighbor)
            new_path: list[Node] = list(path)
            new_path.append(neighbor)
            queue.append(new_path)

    return None # no path found

print(train_soccer(["V1", "V2", "V3", "V4", "V5"])) # Expected: ["V1", "V2", "V3", "V4", "V5"]
print(train_soccer(["V1", "V2", "V1", "V4", "V5"])) # Expected: ["V1", "V4", "V5"]
print(train_soccer(["V1", "V2", "V1", "V2", "V1", "V5"])) # Expected: ["V1", "V5"]
print(train_soccer(["V1", "V2", "V1", "V2", "V1", "V5", "V2"])) # Expected: ["V1", "V2"]