import numpy as np
import logging as log

like_zero = 0.0
warning_ready = True


def sturm_series(p):
    series = [p, np.polyder(p)]
    while series[-1].order > 0:
        next = -np.polydiv(series[-2], series[-1])[1]
        series.append(next)
    return series


def sturm_n(series, x):
    n = 0
    has_sign = False
    wait_for_plus = True
    for p in series:
        if np.isinf(x):
            res = p[p.order] if p.order % 2 == 0 or np.isposinf(x) else -p[p.order]
        else:
            res = p(x)
        if abs(res) > like_zero:
            if not has_sign:
                wait_for_plus = not (res > 0.0)
                has_sign = True
            if wait_for_plus == (res > 0.0):
                n += 1
                wait_for_plus = not wait_for_plus
    return n


def separate_c(a, b):
    if np.isneginf(a):
        if b > 0.0:
            return 0.0
        elif b == 0.0:
            return -20.0
        else:
            return b * 2
    if np.isposinf(b):
        if a < 0.0:
            return 0.0
        elif a == 0.0:
            return 20.0
        else:
            return a * 2
    return (a + b) / 2


sep_iters = 0
def separate_roots(p, a, b, epsilon):
    series = sturm_series(p)
    n_a = sturm_n(series, a)
    n_b = sturm_n(series, b)
    global sep_iters
    sep_iters = 0
    stack = [(a, b, n_a, n_b, sep_iters)]
    while stack:
        a, b, n_a, n_b, sep_iters = stack.pop()
        if n_a - n_b == 1:
            yield (a, b)
            continue
        if b - a < epsilon / 2:
            if warning_ready:
                log.warning(
                    "Warning: separate_roots can't separate roots in [%s %s]. Expected count of roots: %d",
                    a, b, n_a - n_b)
            yield (a, b)
            continue
        sep_iters += 1
        c = separate_c(a, b)
        n_c = sturm_n(series, c)
        if (n_c - n_b) > 0:
            stack.append((c, b, n_c, n_b, sep_iters))
        if (n_a - n_c) > 0:
            stack.append((a, c, n_a, n_c, sep_iters))


loc_iters = 0
def localize_roots(p, a, b, epsilon):
    dp = np.polyder(p); ser_dp = sturm_series(dp)
    ddp = np.polyder(dp); ser_ddp = sturm_series(ddp)
    for (a, b) in separate_roots(p, a, b, epsilon):
        global loc_iters
        loc_iters = 0
        if abs(p(b)) <= like_zero:
            yield (b - epsilon / 3, b)
            continue
        ser_tmp = ser_dp
        n_a = sturm_n(ser_tmp, a)
        n_b = sturm_n(ser_tmp, b)
        while b - a >= epsilon / 2:
            if n_a - n_b <= 0:
                if ser_tmp == ser_dp:
                    ser_tmp = ser_ddp
                    n_a = sturm_n(ser_tmp, a)
                    n_b = sturm_n(ser_tmp, b)
                    continue
                else:
                    break
            loc_iters += 1
            c = separate_c(a, b)
            if abs(p(c)) <= like_zero:
                a = c - epsilon / 6
                b = c - epsilon / 6
                break
            if p(c) * p(b) < 0:
                a = c
                a_n = sturm_n(ser_tmp, a)
            else:
                b = c
                n_b = sturm_n(ser_tmp, b)
        yield (a, b)


fnd_iters = 0
def bfind_roots(p, a, b, epsilon):
    for (a, b) in separate_roots(p, a, b, epsilon):
        global fnd_iters
        fnd_iters = 0
        if abs(p(b)) <= like_zero:
            yield b
            continue
        while b - a >= epsilon / 2:
            fnd_iters += 1
            c = separate_c(a, b)
            if abs(p(c)) <= like_zero:
                break
            if p(c) * p(b) < 0:
                a = c
            else:
                b = c
        yield separate_c(a, b)


def make_soft(p, dp, a, b):
    if dp(a) > dp(b):
        p = -p
        dp = -dp
    if p(a) > p(b):
        c = a
        a = b
        b = c
    return (p, dp, a, b)


crd_iters = 0
def chords(p, a, b, epsilon):
    dp = np.polyder(p)
    for (a, b) in localize_roots(p, a, b, epsilon):
        (p, dp, a, b) = make_soft(p, dp, a, b)
        global crd_iters
        crd_iters = 0
        while abs(b - a) >= epsilon / 4 and abs(p(a) / dp(a)) >= epsilon / 2:
            crd_iters += 1
            a -= p(a) / (p(b) - p(a)) * (b - a)
        yield a


tan_iters = 0
def tangents(p, a, b, epsilon):
    dp = np.polyder(p)
    for (a, b) in localize_roots(p, a, b, epsilon):
        (p, dp, a, b) = make_soft(p, dp, a, b)
        global tan_iters
        tan_iters = 0
        while abs(b - a) >= epsilon / 4 and abs(p(b) / dp(a)) >= epsilon / 2:
            tan_iters += 1
            b -= p(b) / dp(b)
        yield b


com_iters = 0
def combine(p, a, b, epsilon):
    dp = np.polyder(p)
    for (a, b) in localize_roots(p, a, b, epsilon):
        (p, dp, a, b) = make_soft(p, dp, a, b)
        global com_iters
        com_iters = 0
        while abs(b - a) >= epsilon / 4 and abs(p(a) / dp(a)) >= epsilon / 2:
            com_iters += 1
            b -= p(b) / dp(b)
            a -= p(a) / (p(b) - p(a)) * (b - a)
        yield a


def start_test(variant):
    def solve_and_print(p, a, b, epsilon):
        log.info(p)
        i_format = " %10d"
        x_format = " %10.4f"
        global warning_ready
        
        ls_x = []; ls_sep = []; ls_fnd = []
        warning_ready = True
        for x in bfind_roots(p, a, b, epsilon):
            ls_x.append(x)
            ls_sep.append(sep_iters)
            ls_fnd.append(fnd_iters)
        warning_ready = False
        tmp = ["Separate  |" + i_format * len(ls_sep)]
        tmp.extend(ls_sep); log.info(*tmp)
        tmp = ["Bin Find  |" + i_format * len(ls_fnd)]
        tmp.extend(ls_fnd); log.info(*tmp)
        tmp = ["x         |" + x_format * len(ls_x)]
        tmp.extend(ls_x); log.info(*tmp)
        
        ls_x = []; ls_crd = []; ls_loc = [];
        for x in chords(p, a, b, epsilon):
            ls_x.append(x)
            ls_loc.append(loc_iters)
            ls_crd.append(crd_iters)
        tmp = ["Localize  |" + i_format * len(ls_loc)]
        tmp.extend(ls_loc); log.info(*tmp)
        tmp = ["Chords    |" + i_format * len(ls_crd)]
        tmp.extend(ls_crd); log.info(*tmp)
        tmp = ["x         |" + x_format * len(ls_x)]
        tmp.extend(ls_x); log.info(*tmp)
        
        ls_x = []; ls_tan = []
        for x in tangents(p, a, b, epsilon):
            ls_x.append(x)
            ls_tan.append(tan_iters)
        tmp = ["Tangents  |" + i_format * len(ls_tan)]
        tmp.extend(ls_tan); log.info(*tmp)
        tmp = ["x         |" + x_format * len(ls_x)]
        tmp.extend(ls_x); log.info(*tmp)
        
        ls_x = []; ls_com = []
        for x in combine(p, a, b, epsilon):
            ls_x.append(x)
            ls_com.append(com_iters)
        tmp = ["Combine   |" + i_format * len(ls_com)]
        tmp.extend(ls_com); log.info(*tmp)
        tmp = ["x         |" + x_format * len(ls_x)]
        tmp.extend(ls_x); log.info(*tmp)
    
    np.set_printoptions(precision=4, floatmode="fixed")
    log.basicConfig(level=log.INFO, format="%(message)s")
    
    log.info("TASK")
    p = np.poly1d([1.0, -10.2374, -91.2105, 492.560])
    solve_and_print(p, -10, 10, 0.0001)
    
    log.info("EXTREMUM")
    p = np.poly1d([1.0, -4.9, 8.59, -6.255, 1.62, -0.135])
    solve_and_print(p, -np.inf, np.inf, 0.0001)
    
    log.info("NARROW")
    p = np.poly1d([1.0, 0.02, -0.0013, 0.00001,])
    solve_and_print(p, -np.inf, np.inf, 0.0001)
    
    log.info("WIDE")
    p = np.poly1d([1.0, -499, -3321771, -849082949, 765599099678, 14164336630500, -136365493005000])
    solve_and_print(p, -np.inf, 3000, 0.00005)


if __name__=="__main__":
    try:
        start_test(2)
    except ValueError as e:
        log.error("Exception:\n%s\nProgram terminated.", e)
    except KeyboardInterrupt:
        log.error("Program interrupted by user.")
    else:
        log.info("\tTest is done.")
