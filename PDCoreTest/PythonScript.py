#pip install -U scikit-learn
#https://stackoverflow.com/questions/58439444/docker-not-building-image-due-to-not-installing-sklearn

from sklearn import tree
X = [[0, 0], [2, 2]]
y = [0.5, 2.5]
clf = tree.DecisionTreeRegressor()
clf = clf.fit(X, y)
r = clf.predict([[1, 1]])

if not 'canPrint' in locals():
	print(r)
else:
	print('canPrint', '=', canPrint)
