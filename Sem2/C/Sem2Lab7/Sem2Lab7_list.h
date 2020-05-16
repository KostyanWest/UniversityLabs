#ifndef SEM2LAB7_LIST_H
#define SEM2LAB7_LIST_H

typedef struct _Task
{
	struct _Task* pPrev;
	struct _Task* pNext;
	char* question;
	char** answers;
	int answersCount;
	int rightAnswer;
} Task;

void AddTask (Task** const ppFirst, Task* pTask);
Task* GotoTask (Task* pFirst, int number);
void FreeTasks (Task** const ppFirst);

#endif // !SEM2LAB7_LIST_H