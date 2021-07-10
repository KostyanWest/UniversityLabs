import numpy as np
import logging as log
import matplotlib.pyplot as plt

log.basicConfig(level=log.INFO, format="%(message)s")


def tridiagonal_matrix_algorithm(a, b, c, d):
    n = len(a)
    ac, bc, cc, dc = map(np.array, (a, b, c, d))
    xc = []
    for j in range(2, n):
        if(bc[j - 1] == 0):
            ier = 1
            return
        ac[j] = ac[j]/bc[j-1]
        bc[j] = bc[j] - ac[j]*cc[j-1]
    if(b[n-1] == 0):
        ier = 1
        return
    for j in range(2, n):
        dc[j] = dc[j] - ac[j]*dc[j-1]
    dc[n-1] = dc[n-1]/bc[n-1]
    for j in range(n-2, -1, -1):
        dc[j] = (dc[j] - cc[j]*dc[j+1])/bc[j]
    return dc


def Spline(dots, x_var):

    n = len(dots) - 1
    (x, y) = map(list, zip(*dots))
    
    h = [None]
    for i in range(1, n+1):
        h += [ x[i] - x[i-1] ]
    A = [[0.0] * (n) for i in range(n)]
    for i in range(1, n-1):
        A[i+1][i] = h[i+1] / 3
    for i in range(1, n):
        A[i][i] = 2 * (h[i] + h[i+1]) / 3
    for i in range(1, n-1):
        A[i][i+1] = h[i+1] / 3
    F = []
    for i in range(1, n):
        F += [( (y[i+1] - y[i]) / h[i+1] - (y[i] - y[i-1]) / h[i]) ]
    A = [A[i][1:] for i in range(len(A)) if i]

    A1 = []
    A2 = []
    A3 = []

    for i in range(n-2):
        A1 += [A[i+1][i]]
        A3 += [A[i][i+1]]
    for i in range(n-1):
        A2 += [A[i][i]]
    c = tridiagonal_matrix_algorithm(A1, A2, A3, F)
    c = [0.0] + list(c) + [0.0]

    def evaluate(x_var):
        for i in range(1, len(x)):
            if x[i-1] <= x_var <= x[i]:
                val = 0
                val += y[i]
                b = (y[i] - y[i-1]) / h[i] + (2 * c[i] + c[i-1]) * h[i] / 3
                val += b * (x_var - x[i])
                val += c[i] * ((x_var - x[i]) ** 2)
                d = (c[i] - c[i-1]) / (3 * h[i])
                val += d * ((x_var - x[i]) ** 3)
                return val
        return None
    
    return evaluate


def start_test(variant):
    def solve_and_print(f, start, end, count, x_var):
        dots = [(x, f(x)) for x in np.linspace(start, end, count)]
        log.info("(x,y) = %s\n", dots)
        (x, y) = map(list, zip(*dots))
        spl = Spline(dots, x_var)

        log.info("f(%.4f) = %.4f", x_var, f(x_var))
        log.info("S(%.4f) = %.4f", x_var, spl(x_var))
        minus = abs(f(x_var) - spl(x_var))
        log.info("S(%.4f) - f(%.4f) = %.4f", x_var, x_var, minus)

        plotdots = 10**4
        plt.plot(x, y, 'og')
        xplot = np.linspace(min(x), max(x), plotdots)
        yplot = [f(xdot) for xdot in xplot]
        plt.plot(xplot, yplot, 'black')
        yplot = [spl(xdot) for xdot in xplot]
        plt.plot(xplot, yplot, 'b')
        plt.show()

    log.info("TASK")
    solve_and_print(np.arctan, 0, 2, 6, 1.)


if __name__=="__main__":
    try:
        start_test(11)
    except KeyboardInterrupt:
        log.error("Program interrupted by user.")
    else:
        log.info("Test is done.")
