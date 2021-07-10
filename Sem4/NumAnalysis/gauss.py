import numpy as np
import logging as log

log.basicConfig(level=log.INFO, format="%(message)s")


def gauss_forward(A):
    indices = np.arange(0, len(A))
    for column in range(0, len(A) - 1):
        valid = A[column:, column] != 0
        if valid.any():
            i_nonzero = indices[column:][valid][0]
            if i_nonzero != column:
                A[[column, i_nonzero]] = A[[i_nonzero, column]]
        else:
            raise ValueError(
                f"There isn't any non-zero number at the {column} column."
                )
        q = A[column + 1: ,column] / A[column, column]
        A[column + 1:, column:] -= np.outer(q, A[column, column:])


def gauss_backward(A):
    for column in range(len(A) - 1, -1, -1):
        A[column, -1] /= A[column, column]
        A[:column, -1] -= A[:column, column] * A[column, -1]
        A[:column, column] = np.zeros(column) # unnecessary zeroing
        A[column, column] = 1 # unnecessary zeroing


def gauss(A):
    gauss_forward(A)
    gauss_backward(A)
    return A[:, -1]


def gauss_column(A):
   for column in range(0, len(A) - 1):
       i_max = abs(A[column:, column]).argmax()
       A[[column, i_max]] = A[[i_max, column]]
   return gauss(A)
       


def gauss_full(A):
    indices = np.arange(0, len(A))
    for column in range(0, len(A) - 1):
        M = abs(A[column:, column:-1])
        j_max = M.max(axis=0).argmax()
        i_max = M[:, j_max].argmax()
        j_max += column
        i_max += column
        A[[column, i_max]] = A[[i_max, column]]
        A[:, [column, j_max]] = A[:, [j_max, column]]
        indices[[column, j_max]] = indices[[j_max, column]]
    x = gauss(A)
    x[indices] = x[np.arange(0, len(x))]
    return x


def makeGilb(n):
    return lambda i, j: 1. / ((n + n - 1) - (i + j))


def makeD(arr):
    return lambda i, j: arr[np.array(j - i, int)]


def start_test(variant):
    np.set_printoptions(precision=2)
    b = np.ones(5, dtype=float) * 4.2
    C = (np.eye(5, k=2, dtype=float) +
         np.eye(5, k=0, dtype=float) +
         np.eye(5, k=-2, dtype=float)) * 0.2
    
    # Gauss
    log.info("GAUSS")
    D = np.fromfunction(
        makeD(np.array([2.33, 0.81, 0.67, 0.92, -0.53], float)),
        (5, 5)
        )
    A = variant * C + D
    log.info("b =\n%s\nC =\n%s\nD =\n%s\nA = %s * C + D =\n%s",
        b, C, D, variant, A)
    ext = np.concatenate((A, b[:,np.newaxis]), axis=1)
    ext1 = ext.copy()
    x1 = gauss(ext1)
    ext2 = ext.copy()
    x2 = gauss_column(ext2)
    ext3 = ext.copy()
    x3 = gauss_full(ext3)
    log.info("x1 =\n%s\nx2 =\n%s\nx3 =\n%s", x1, x2, x3)
    
    # Gilbert
    log.info("GILBERT")
    b = np.ones(8, dtype=float) * 10.5
    A = np.fromfunction(makeGilb(8), (8, 8))
    log.info("A for Gilbert =\n%s", A)
    ext = np.concatenate((A, b[:,np.newaxis]), axis=1)
    ext1 = ext.copy()
    x1 = gauss(ext1)
    ext2 = ext.copy()
    x2 = gauss_column(ext2)
    ext3 = ext.copy()
    x3 = gauss_full(ext3)
    log.info("x1 =\n%s\nx2 =\n%s\nx3 =\n%s", x1, x2, x3)
    xx = np.array([540540, -1765764, 2270268, -1455300, 485100, -79380, 5292, -84],float)
    log.info("d(x1) =\n%s", abs(xx - x1))
    log.info("d(x2) =\n%s", abs(xx - x2))
    log.info("d(x3) =\n%s", abs(xx - x3))


if __name__=="__main__":
    try:
        start_test(11)
    except ValueError as e:
        log.error("Exception:\n%s\nProgram terminated.", e)
    except KeyboardInterrupt:
        log.error("Program interrupted by user.")
    else:
        log.info("Test is done.")
