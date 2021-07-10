import numpy as np
import logging as log

log.basicConfig(level=log.INFO, format="%(message)s")


def norm1(B):
    if B.ndim > 1:
        return abs(B).sum(axis=0).max()
    else:
        return abs(B).max()


def norm2(B):
    return np.sqrt((B * B).sum())


def norm3(B):
    if B.ndim > 1:
        return abs(B).sum(axis=1).max()
    else:
        return abs(B).sum()


def max_no_diag(A):
    absolute_A = np.absolute(A)
    current_max = absolute_A[0][1]
    i, j = 0, 1
    for row in range(len(A)):
        for col in range(row + 1, len(A)):
            if absolute_A[row][col] > current_max:
                i, j, current_max = row, col, absolute_A[row][col]
    return (i,j)


def jacobi_rotation(A, eps):
    iterations = 0
    V = np.eye(len(A))
    while(True):
        iterations += 1
        i, j = max_no_diag(A)

        if (A[i][i] == A[j][j]):
       	    p = np.pi / 4
        else:
            p = 2 * A[i][j] / (A[i][i] - A[j][j])
        co = np.cos(1/2 * np.arctan(p))
        si = np.sin(1/2 * np.arctan(p))
        V_k = np.eye(len(A))
        V_k[i][i] = co
        V_k[i][j] = -si
        V_k[j][i] = si
        V_k[j][j] = co
        A = np.transpose(V_k).dot(A).dot(V_k)
        V = V.dot(V_k)
        if norm1(A - np.diag(np.diag(A))) < eps:
            break
    return(A, V, iterations)


def print_matrix(A):
    for row in A:
        log.info("%8.4f" * len(row), *row)


def print_vector(v):
    log.info("[ " + "%8.4f" * len(v) + " ]", *v)


def start_test(variant):
    def solve_and_print(A, eps):
        A, V, iterations = jacobi_rotation(A, eps)
        log.info("A (after %d iterations) =", iterations)
        print_matrix(A)
        log.info("\n||non_diag(A)|| = %.6f (eps = %.6f)", norm1(A - np.diag(np.diag(A))), eps)
        log.info("\neigenvalues =")
        print_vector(np.diag(A))
        log.info("\neigenvectors =")
        for v in V.T:
            print_vector(v)


    np.set_printoptions(precision=4)

    # Task
    log.info("TASK")
    C = (np.eye(5, k=0, dtype=float) * 0.2 +
         np.eye(5, k=2, dtype=float) * 0.2 +
         np.eye(5, k=-2, dtype=float) * 0.2)
    D = np.array([
        [2.33, 0.81, 0.67, 0.92, -0.53],
        [0.81, 2.33, 0.81, 0.67, 0.92],
        [0.67, 0.81, 2.33, 0.81, 0.92],
        [0.92, 0.67, 0.81, 2.33, -0.53],
        [-0.53, 0.92, 0.92, -0.53, 2.33]
    ])
    A = variant * C + D

    log.info("C = ")
    print_matrix(C)
    log.info("\nD = ")
    print_matrix(D)
    log.info("\nA = ")
    print_matrix(A)
    log.info("")
    solve_and_print(A, 0.0001)

    # Example 1
    log.info("\nEXAMPLE 1")
    A = np.array([
        [-3, 5, 7],
        [5, 4, -1],
        [7, -1, 0]
    ])
    log.info("A = ")
    print_matrix(A)
    log.info("")
    solve_and_print(A, 0.0001)

    # Example 2
    log.info("GILBERT")
    A = np.fromfunction(lambda i, j: 1. / (i + j + 1), (5, 5))
    log.info("\nA = ")
    print_matrix(A)
    log.info("")
    solve_and_print(A, 0.0001)


if __name__=="__main__":
    try:
        start_test(11)
    except KeyboardInterrupt:
        log.error("Program interrupted by user.")
    else:
        log.info("Test is done.")
