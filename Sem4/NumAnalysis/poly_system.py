import numpy as np
import logging as log
import time

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


def iteration(fx, fy, x0, y0):
    global ITERATIONS
    ITERATIONS = 0
    (x, y) = (x0, y0)
    while True:
        ITERATIONS += 1
        old_x = x
        old_y = y
        x = fx(x, y)
        y = fy(x, y)
        if not (np.isfinite(x) and np.isfinite(y)):
            raise RuntimeError("Sequence {x} is divergent")
        if max(abs(x - old_x), abs(y - old_y)) < EPS:
            return x, y


def newton(f1, f2, J, x0, y0):
    global ITERATIONS
    ITERATIONS = 0
    (x, y) = (x0, y0)
    while True:
        ITERATIONS += 1
        j = J(x, y)
        f = np.array([[f1(x, y)], [f2(x, y)]])
        x_x0 = np.linalg.solve(j, -f)
        x += x_x0[0][0]
        y += x_x0[1][0]
        if not (np.isfinite(x) and np.isfinite(y)):
            raise RuntimeError("Sequence {x} is divergent")
        if norm1(x_x0) < EPS:
            return x, y


def mnewton(f1, f2, J, x0, y0):
    global ITERATIONS
    ITERATIONS = 0
    (x, y) = (x0, y0)
    j = J(x, y)
    while True:
        ITERATIONS += 1
        f = np.array([[f1(x, y)], [f2(x, y)]])
        x_x0 = np.linalg.solve(j, -f)
        x += x_x0[0][0]
        y += x_x0[1][0]
        if not (np.isfinite(x) and np.isfinite(y)):
            raise RuntimeError("Sequence {x} is divergent")
        if norm1(x_x0) < EPS:
            return x, y


def start_test(variant):
    def solve_and_print(f1, f2, fx, fy, J, x0, y0):
        global ITERATIONS
        
        log.info("%10s|%10s%10s%10s%15s", "", "x", "y", "iters", "time")
        log.info("%s|%s", "-" * 10, "-" * 45)

        ITERATIONS = 0
        t1 = time.time()
        for _ in range(1000):
            result = iteration(fx, fy, x0, y0)
        t2 = time.time()
        log.info("%10s|%10.4f%10.4f%10d%15.10f", "iteration", result[0], result[1], ITERATIONS, (t2-t1)/1000)

        ITERATIONS = 0
        t1 = time.time()
        for _ in range(1000):
            result = newton(f1, f2, J, x0, y0)
        t2 = time.time()
        log.info("%10s|%10.4f%10.4f%10d%15.10f", "newton", result[0], result[1], ITERATIONS, (t2-t1)/1000)

        ITERATIONS = 0
        t1 = time.time()
        for _ in range(1000):
            result = mnewton(f1, f2, J, x0, y0)
        t2 = time.time()
        log.info("%10s|%10.4f%10.4f%10d%15.10f", "mnewton", result[0], result[1], ITERATIONS, (t2-t1)/1000)


    global m
    m = 0.3
    global a
    a = 1.0
    global EPS
    EPS = 0.0001
    global ITERATIONS

    # Task.1
    x0 = 0.2; y0 = -0.7
    log.info("TASK.1\nx0 = %.4f\ny0 = %.4f", x0, y0)
    solve_and_print(
        lambda x, y: np.tan(x * y + m) - x,
        lambda x, y: a * (x ** 2) + 2 * (y ** 2) - 1,
        lambda x, y: np.tan(x * y + m),
        lambda x, y: -np.sqrt((1 - a * (x ** 2)) / 2),
        lambda x, y: np.array([
            [(1 + np.tan(x * y + m) ** 2) * y - 1, (1 + np.tan(x * y + m) ** 2) * x],
            [2 * a * x, 4 * y]]
        ),
        x0,
        y0
    )

    # Task.2
    x0 = 0.7; y0 = 0.5
    log.info("TASK.2\nx0 = %.4f\ny0 = %.4f", x0, y0)
    solve_and_print(
        lambda x, y: np.tan(x * y + m) - x,
        lambda x, y: a * (x ** 2) + 2 * (y ** 2) - 1,
        lambda x, y: np.tan(x * y + m),
        lambda x, y: np.sqrt((1 - a * (x ** 2)) / 2),
        lambda x, y: np.array([
            [(1 + np.tan(x * y + m) ** 2) * y - 1, (1 + np.tan(x * y + m) ** 2) * x],
            [2 * a * x, 4 * y]]
        ),
        x0,
        y0
    )

    # Example 1
    x0 = 1.0; y0 = 1.0
    log.info("EXAMPLE1\nx0 = %.4f\ny0 = %.4f", x0, y0)
    solve_and_print(
        lambda x, y: 3 * x * x - 2 * y - 2,
        lambda x, y: np.sin(x) - y,
        lambda x, y: np.sqrt((y + 1) * 2 / 3),
        lambda x, y: np.sin(x),
        lambda x, y: np.array([
            [6 * x, -2],
            [np.cos(x), -1]]
        ),
        x0,
        y0
    )

    # Example 2
    x0 = 0.5; y0 = 2.0
    log.info("EXAMPLE2\nx0 = %.4f\ny0 = %.4f", x0, y0)
    solve_and_print(
        lambda x, y: x * y - y + 1,
        lambda x, y: 5 * x - y * y + 2,
        lambda x, y: 1 - 1 / y,
        lambda x, y: np.sqrt(5 * x + 2),
        lambda x, y: np.array([
            [y, x - 1],
            [5, -2 * y]]
        ),
        x0,
        y0
    )


if __name__=="__main__":
    try:
        start_test(12)
    except KeyboardInterrupt:
        log.error("Program interrupted by user.")
    else:
        log.info("\tTest is done.")
