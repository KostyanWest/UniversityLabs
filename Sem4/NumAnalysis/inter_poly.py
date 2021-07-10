import numpy as np
import logging as log

log.basicConfig(level=log.INFO, format="%(message)s")


def omega(xs):
    p = np.poly1d([1])
    for x in xs:
        p *= np.poly1d([1, -x])

    def omega_i(index=None):
        if index is None:
            return p
        else:
            return (p / np.poly1d([1, -xs[index]]))[0]

    return omega_i


def lagrange(xs, ys):
    L = np.poly1d([0])
    w = omega(xs)
    for i in range(len(xs)):
        L += w(i) / w(i)(xs[i]) * ys[i]
    return L


def f(xs, ys, i, j):
    if i == j:
        return ys[i]
    else:
        return (f(xs, ys, i+1, j) - f(xs, ys, i, j-1)) / (xs[j] - xs[i])


def newton(xs, ys):
    N = 0
    w = np.poly1d([1])
    for i in range(len(xs)):
        N += w * f(xs, ys, 0, i)
        w *= np.poly1d([1, -xs[i]])
    return N


def start_test(variant):
    def solve_and_print(xs, ys):
        log.info("L (lagrange) =")
        L = lagrange(xs, ys)
        log.info("%s", L)
        log.info("L(0.47) = %.4f", L(0.47))
        log.info("\nN (newton) =")
        N = newton(xs, ys)
        log.info("%s", L)
        log.info("N(0.47) = %.4f", N(0.47))

    np.set_printoptions(precision=4)

    # Task
    log.info("TASK")
    xs = np.linspace(0., 1., 11)
    m = -2.53
    ps = np.array([0.0, 0.41, 0.79, 1.13, 1.46, 1.76, 2.04, 2.3, 2.55, 2.79, 3.01], float)
    ys = ps + m
    solve_and_print(xs, ys)

    # Example 1
    log.info("\nEXAMPLE 1")
    xs = [-2., -1., 0., 1., 2.]
    ys = [3., 0., -1., 0., 3.]
    solve_and_print(xs, ys)

    # Exapmle 2
    log.info("\nEXAMPLE 2")
    xs = [0., np.pi / 2, np.pi, np.pi * 3 / 2, np.pi * 2]
    ys = [0., 1., 0., -1., 0.]
    solve_and_print(xs, ys)


if __name__=="__main__":
    try:
        start_test(11)
    except KeyboardInterrupt:
        log.error("Program interrupted by user.")
    else:
        log.info("Test is done.")
