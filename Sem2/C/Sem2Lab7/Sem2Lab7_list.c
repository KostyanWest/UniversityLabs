#include <stdlib.h>
#include "Sem2Lab7_list.h"

void AddTask (Task** const ppFirst, Task* pTask)
{
	if (*ppFirst != NULL) {
		Task* pTemp = *ppFirst;
		while (pTemp->pNext != NULL) {
			pTemp = pTemp->pNext;
		}
		pTemp->pNext = pTask;
		pTask->pPrev = pTemp;
		pTask->pNext = NULL;
	} else {
		*ppFirst = pTask;
		pTask->pPrev = NULL;
		pTask->pNext = NULL;
	}
}

Task* GotoTask (Task* pFirst, int number)
{
	Task* pTemp = pFirst;
	while (number > 0 && pTemp != NULL) {
		pTemp = pTemp->pNext;
		number--;
	}
	return pTemp;
}

void FreeTasks (Task** const ppFirst)
{
	Task* pTemp = *ppFirst;
	while (pTemp != NULL) {
		Task* pTemp2 = pTemp;
		if (pTemp->question != NULL) {
			free(pTemp->question);
		}
		if (pTemp->answers != NULL) {
			for (int i = 0; i < pTemp->answersCount; i++) {
				free(pTemp->answers[i]);
			}
			free(pTemp->answers);
		}
		pTemp = pTemp->pNext;
		free(pTemp2);
	}
	*ppFirst = NULL;
}