from random import randint
from copy import deepcopy
import sys

def gen_level(x, y, min_inversions, start, goal):
    matrix = gen_rand_matrix(x, y)
    (s_x, s_y) = start
    matrix[s_x][s_y] = 0
    (g_x, g_y) = goal
    matrix[g_x][g_y] = 0

    while not solvable(matrix, min_inversions, start, goal):
        matrix = gen_rand_matrix(x, y)
        matrix[s_x][s_y] = 0
        matrix[g_x][g_y] = 0

    matrix[s_x][s_y] = 2
    matrix[g_x][g_y] = 3
    return matrix

def gen_rand_matrix(x, y):
    return [[randint(0, 1) for n in xrange(y)] for m in xrange(x)]

def solvable(matrix, min_inversions, start, goal):
    visited = { start }
    matrix_normal = matrix
    matrix_inverted = invert_matrix(matrix, goal)
    rows = len(matrix)
    cols = len(matrix[0])
    valid_queue = [ start ]
    lava_queue = []
    inversions = 0

    while valid_queue or lava_queue:
        if not valid_queue:
            inversions += 1
            if inversions > min_inversions:
                return False
            if inversions % 2 == 0:
                matrix = matrix_normal
            else:
                matrix = matrix_inverted

            valid_queue = lava_queue
            lava_queue = []

        node = valid_queue.pop()
        visited.add(node)
        if node == goal:
            if inversions < min_inversions:
                return False
            else:
                return True

        (x, y) = node

        if is_valid((x - 1, y), rows, cols, visited):
            if matrix[x - 1][y] == 0:
                valid_queue.append((x - 1, y))
            else:
                lava_queue.append((x - 1, y))
        if is_valid((x + 1, y), rows, cols, visited):
            if matrix[x + 1][y] == 0:
                valid_queue.append((x + 1, y))
            else:
                lava_queue.append((x + 1, y))
        if is_valid((x, y - 1), rows, cols, visited):
            if matrix[x][y - 1] == 0:
                valid_queue.append((x, y - 1))
            else:
                lava_queue.append((x, y - 1))
        if is_valid((x, y + 1), rows, cols, visited):
            if matrix[x][y + 1] == 0:
                valid_queue.append((x, y + 1))
            else:
                lava_queue.append((x, y + 1))

    return False

def is_valid((x, y), rows, cols, visited):
    return 0 <= x and x < rows and 0 <= y and y < cols and (x, y) not in visited

def invert_matrix(matrix, (g_x, g_y)):
    matrix_inverted = deepcopy(matrix)
    orig_goal_val = matrix[g_x][g_y]

    for row in xrange(len(matrix)):
        for col in xrange(len(matrix[0])):
            matrix_inverted[row][col] = 1 - matrix[row][col]
    matrix_inverted[g_x][g_y] = orig_goal_val

    return matrix_inverted

def print_level(matrix):
    for row in xrange(len(matrix)):
        for col in xrange(len(matrix[0])):
            print(str(matrix[row][col])),
        print

if __name__ == '__main__':
    rows = int(sys.argv[1])
    cols = int(sys.argv[2])
    min_inversions = int(sys.argv[3])

    start = (0, cols - 1)
    goal = (rows - 1, 0)
    print_level(gen_level(rows, cols, min_inversions, start, goal))
