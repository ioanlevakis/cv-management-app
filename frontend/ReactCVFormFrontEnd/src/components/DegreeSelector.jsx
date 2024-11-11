import React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faGraduationCap } from '@fortawesome/free-solid-svg-icons';

const DegreeSelector = ({ index, degree, handleChange, options }) => {
    return (
        <div className="inputDiv">
            <FontAwesomeIcon icon={faGraduationCap} className="inputIcon" />
            <select value={degree} onChange={(e) => handleChange(index, e.currentTarget.value)}>
                <option value="">Please select a Degree</option>
                {options.map(option => (
                    <option key={option} value={option}>{option}</option>
                ))}
            </select>
        </div>
    );
};

export default DegreeSelector;
