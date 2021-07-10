from matplotlib import pylab
from scipy.integrate import odeint as od
import numpy as np

EPS = 0.001

a = 0
b = 1
y0 = 0


def to_fixed(numObj, digits=0):
    if isinstance(numObj, list):
        return [to_fixed(i, digits) for i in numObj]
    return f"{numObj:.{digits}f}"


def f(y, x):
    return (1.3 * (1 - (y ** 2))) / ((1 + 2.0) * (x ** 2) + (y ** 2) + 1)


def runge_kutt(xi, yi, h):
    h2 = h / 2
    k1 = f(yi, xi)
    k2 = f(yi + h2 * k1, xi + h2)
    k3 = f(yi + h2 * k2, xi + h2)
    k4 = f(yi + h * k3, xi + h)
    return yi + (h / 6) * (k1 + 2 * k2 + 2 * k3 + k4)
    
def runge_kutta(x, step):
    if x <= a:
        return y0
    prev_x = x - step
    prev_y = runge_kutta(prev_x, step)
    k1 = f(prev_y, prev_x)
    k2 = f(prev_y + (step / 2) * k1, prev_x + step / 2)
    k3 = f(prev_y + (step / 2) * k2, prev_x + step / 2)
    k4 = f(prev_y + step * k3, x)
    return prev_y + step * (k1 + 2 * k2 + 2 * k3 + k4) / 6


def find_step():
    h0 = EPS ** (1 / 4)
    n = int((b - a) // h0)
    if n % 2 != 0:
        n += 1
    while check_step(n):
        n = n // 4 * 2
    while not check_step(n):
        n += 2
    return (b - a) / n


def check_step(n):
    h = (b - a) / n
    y2 = runge_kutta(a + 2 * h, h)
    y2e = runge_kutta(a + 2 * h, h * 2)
    eps = (1 / 15) * abs(y2 - y2e)
    return eps < EPS


def exact(x):
    sol = od(f, y0, [a, x])
    return sol[1][0]

def next_y(xi, yi, h):
    h2 = h / 2
    delta_y = h * f(yi + h2 * f(yi, xi), xi + h2)
    return yi + delta_y

def euler(xs, step):
    ys = []
    y = y0
    for x in xs:
        ys.append(y)
        y = next_y(x, y, step)
    return ys


def start_test(variant):
    print("Исходные данные:")
    print(f"y = (0.7*(1-(y**2)))/((1+2.0)*(x**2)+(y**2)+1)")
    print(f"y(0) = {y0}")
    print(f"\nИнтервал: [{a}, {b}]")
    print(f"Погрешность: {EPS}")
    print()
    step = find_step()
    print("Шаг итерирования: ", step)
    xlist = np.arange(a, b + step, step)
    runge_kutta_points = []
    euler_points = []
    exact_points = []
    euler_points = euler(xlist, step)
    x = a
    while x <= b:
        r2 = exact(x)
        exact_points.append(r2)
        x += step
    y1 = y0
    for x in xlist:
        runge_kutta_points.append(y1)
        y1 = runge_kutt(x, y1, step)
        
    print(f"Значения функции в точках методом Элера: {to_fixed(euler_points, 4)}")
    print(f"Значения функции в точках методом Рунге-Кутта: {to_fixed(runge_kutta_points, 4)}")
    print(f"Точные значения функции: {to_fixed(exact_points, 4)}")    
    pylab.cla()
    pylab.plot (xlist, exact_points, label = "точное решение", color = (0, 1, 0))
    pylab.plot (xlist, euler_points, label = "кривая методом Эйлера", color = (1, 0, 0))
    pylab.plot (xlist, runge_kutta_points, label = "кривая методом Рунге-Кутта ", color = (0, 0, 1))
    pylab.grid(True)
    pylab.legend()
    pylab.savefig("lab9.png")
    pylab.show()


if __name__=="__main__":
    try:
        start_test(12)
    except KeyboardInterrupt:
        print("Program interrupted by user.")
    else:
        print("\tTest is done.")
