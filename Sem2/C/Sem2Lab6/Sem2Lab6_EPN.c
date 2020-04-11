#include <stdio.h>
#include <stdlib.h>

void EnterPositiveNum(int* pN)
{
	int n;
	char ch = 0;
	int correct;

	while (1) {
		correct = scanf_s("%5d%c", &n, &ch, 1);
		if (ch != '\n') {
			scanf_s("%*[^\n]"); /* Clearing the buffer */
		}
		if (correct > 0 && (ch == '\n' || ch == ' ') && n > 0) {
			/* Set value */
			*pN = n;
			return;
		}
		printf("-> Invalid input, try again:\n");
	}
}