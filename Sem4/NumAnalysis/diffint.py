import numpy as np
import logging as log
import time

log.basicConfig(level=log.INFO, format="%(message)s")


def diff_1(f, x, h):
    return (f(x + h) - f(x - h)) / (2 * h)


def diff_2(f, x, h):
    return (f(x + h) - 2 * f(x) + f(x - h)) / h ** 2


def int_rectangle(f, a, b, n):
    h = (b - a) / n
    h2 = h / 2
    x = np.linspace(a, b, n + 1)
    x += h2
    y = f(x)
    return y[:-1].sum() * h


def int_trapezoid(f, a, b, n):
    h = (b-a)/n
    x = np.linspace(a, b, n + 1)
    y = f(x)
    return (y[1:-1].sum() + (y[0] + y[-1])/2) * h


def int_simpson(f, a, b, n):
    h = (b - a) / (n * 2)
    x = np.linspace(a, b, n * 2 + 1)
    y = f(x)
    return (y[1:-1:2].sum() * 4 + y[2:-2:2].sum() * 2 + y[0] + y[-1]) * h / 3


def f(x):
    return np.sqrt(1. - np.log10(x) ** 2)


def start_test(variant):
    def solve_and_print(f, x, a, b):
        log.info("f'(%.2f)  = %.6f", x, diff_1(f, x, 1/10))
        log.info("f''(%.2f) = %.6f", x, diff_2(f, x, 1/10))
        t1 = time.time()
        for _ in range(1000):
            result = int_rectangle(f, a, b, 6000)
        t2 = time.time()
        log.info("int_rectangle = %.6f, time = %.4f, n = 6000", result, (t2-t1))
        t1 = time.time()
        for _ in range(1000):
            result = int_trapezoid(f, a, b, 6000)
        t2 = time.time()
        log.info("int_trapezoid = %.6f, time = %.4f, n = 6000", result, (t2-t1))
        t1 = time.time()
        for _ in range(1000):
            result = int_simpson(f, a, b, 3000)
        t2 = time.time()
        log.info("int_simpson   = %.6f, time = %.4f, n = 3000", result, (t2-t1))
        t1 = time.time()

    log.info("EXAMPLE 2")
    solve_and_print(f, 5.5, 1., 10.)
    pass


if __name__=="__main__":
    try:
        start_test(6)
    except KeyboardInterrupt:
        log.error("Program interrupted by user.")
    else:
        log.info("\tTest is done.")
