import numpy as np
import logging as log

log.basicConfig(level=log.INFO, format="%(message)s")


def to_iterable(A, b):
    return np.eye(len(A)) - A, b


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


def is_convergeable_by_iteration(B):
    norm_by = [norm1, norm2, norm3]
    norms = np.array([norm1(B), norm2(B), norm3(B)], float)
    return norms.min() < 1, norm_by[norms.argmin()]


def is_convergeable_by_seidel(B):
    norm_by = [norm1, norm3]
    norms = np.array([norm1(B), norm3(B)], float)
    return norms.min() < 1, norm_by[norms.argmin()]


def iteration_cycle(A, b, epsilon, isSeidel):
    B, c = to_iterable(A, b)
    if isSeidel:
        is_convergeable, get_norm = is_convergeable_by_seidel(B)
    else:
        is_convergeable, get_norm = is_convergeable_by_iteration(B)
    if not is_convergeable:
        raise ValueError(
        f"{'Seidel' if isSeidel else 'Iteration'} method isn't converge! " +
        f"The best norm is {get_norm(B)} with the norm function \"{get_norm.__name__}\"."
        )
    B_norm = get_norm(B) / (1 - get_norm(B))
    get_norm = norm1
    x = c.copy()

    iteration = 0
    converged = False
    while not converged:
        iteration += 1
        if isSeidel:
            x_new = x.copy()
            for i in range(len(B)):
                x_new[i] = (B[i] * x_new).sum() + c[i]
        else:
            x_new = B @ x + c
        converged = B_norm * get_norm(x_new - x) < epsilon
        x = x_new
    return x, iteration, get_norm(B)


def iteration(A, b, epsilon):
    return iteration_cycle(A, b, epsilon, False)


def seidel(A, b, epsilon):
    return iteration_cycle(A, b, epsilon, True)


def makeD(arr):
    return lambda i, j: arr[np.array(j - i, int)]


def start_test(variant):
    np.set_printoptions(precision=4)
    b = np.array([1.2, 2.2, 4.0, 0.0,-1.2], float)
    C = (np.eye(5, k=0, dtype=float) +
         np.eye(5, k=-1, dtype=float)) * 0.01
    C[0, 2] = C[1, 2] = C[2, 4] = -0.02
    D = np.fromfunction(
        makeD(np.array([1.33, 0.21, 0.17, 0.12, -0.13], float)), (5, 5)
        )
    A = variant * C + D

    # Iteration
    log.info("ITERATION")
    log.info("b =\n%s\nC =\n%s\nD =\n%s\nA = %s * C + D =\n%s",
        b, C, D, variant, A)

    res, it, Bnorm = iteration(A, b, 0.0001)
    log.info("x_iteration =\n%s\n(with %d iterations, ||B|| = %.4f)", res, it, Bnorm)
    res2, it, Bnorm = seidel(A, b, 0.0001)
    log.info("x_seidel =\n%s\n(with %d iterations, ||B|| = %.4f)", res2, it, Bnorm)

    # Example 1
    log.info("EXAMPLE1")
    b = np.ones(8, dtype=float) * 2
    A = (np.eye(8, k=0, dtype=float) * 0.8 +
         np.eye(8, k=4, dtype=float) * 0.2 +
         np.eye(8, k=-6, dtype=float) * 0.1)
    log.info("b =\n%s\nA =\n%s", b, A)

    res, it, Bnorm = iteration(A, b, 0.0001)
    log.info("x_iteration =\n%s\n(with %d iterations, ||B|| = %.4f)", res, it, Bnorm)
    res2, it, Bnorm = seidel(A, b, 0.0001)
    log.info("x_seidel =\n%s\n(with %d iterations, ||B|| = %.4f)", res2, it, Bnorm)

    # Example 2
    log.info("EXAMPLE2")
    b = np.array([1.0, 1.1, 1.2, 1.3, 1.4], float)
    A = (np.eye(5, k=0, dtype=float) +
         np.eye(5, k=-1, dtype=float) * 0.1 +
         np.eye(5, k=-2, dtype=float) * 0.1 +
         np.eye(5, k=-3, dtype=float) * 0.1 +
         np.eye(5, k=-4, dtype=float) * 0.1)
    log.info("b =\n%s\nA =\n%s", b, A)

    res, it, Bnorm = iteration(A, b, 0.0001)
    log.info("x_iteration =\n%s\n(with %d iterations, ||B|| = %.4f)", res, it, Bnorm)
    res2, it, Bnorm = seidel(A, b, 0.0001)
    log.info("x_seidel =\n%s\n(with %d iterations, ||B|| = %.4f)", res2, it, Bnorm)


if __name__=="__main__":
    try:
        start_test(11)
    except ValueError as e:
        log.error("Exception:\n%s\nProgram terminated.", e)
    except KeyboardInterrupt:
        log.error("Program interrupted by user.")
    else:
        log.info("Test is done.")
