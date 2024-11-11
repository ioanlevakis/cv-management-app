import React from 'react'
import { useState, useEffect } from 'react'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faAddressCard, faUser, faAt, faPhone, faFileLines, faCircleNotch } from '@fortawesome/free-solid-svg-icons'
import axios from 'axios'
import DOMPurify from 'dompurify'
import '../styles/CVFormStyles.css'
import DegreeSelector from './DegreeSelector';


axios.defaults.baseURL = import.meta.env.VITE_API_URL;
axios.defaults.withCredentials = true;


export default function CVForm() {
    const [csrfToken, setCsrfToken] = useState(null);
    const [candidateId, setCandidateId] = useState(null);
    const [lastname, setLastname] = useState('');
    const [firstname, setFirstname] = useState('');
    const [email, setEmail] = useState('');
    const [mobile, setMobile] = useState('');
    const [degrees, setDegrees] = useState(['']);
    const [file, setFile] = useState(null);
    const [loading, setLoading] = useState(false);
    const [isFileUploaded, setIsFileUploaded] = useState(null);
    const [isSubmitSuccessful, setIsSubmitSuccessful] = useState('Submission required');

    const fetchCsrfToken = async () => {
        try {
            const response = await axios.get('/Candidates');
            setCsrfToken(response.data.csrfToken);
        } catch (error) {
            console.error('Error fetching CSRF token:', error.data);
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchCsrfToken();
    }, []);

    const sanitizeInput = (input) => DOMPurify.sanitize(input);

    const handleFile = (e) => {

        const uploadedFile = e.currentTarget.files[0];

        if (uploadedFile) {

            const fileType = uploadedFile.type;
            const acceptTypes = [
                'application/pdf',
                'application/msword',
                'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
            ];

            if (acceptTypes.includes(fileType)) {
                setFile(uploadedFile);
                setIsFileUploaded('File successfully uploaded')
            }
            else {
                setIsFileUploaded('Please upload a PDF or a Word document.');
                setFile(null);
            }
        }
        else {
            setIsFileUploaded('No file selected');
            setFile(null);
        }
    };


    const validateInput = () => {

        const candidateNameRegex = /^[a-zA-Z]{1,15}$/;

        if (!candidateNameRegex.test(firstname)) {
            setIsSubmitSuccessful("Firstname accepts maximum 15 English letters");
            return;
        }

        if (!candidateNameRegex.test(lastname)) {
            setIsSubmitSuccessful("Lastname accepts maximum 15 English letters");
            return;
        }

        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/

        if (!emailRegex.test(email)) {
            setIsSubmitSuccessful("Invalid email address format");
            return;
        }

        return true;
    };

    const handleDelete = async () => {
        if (candidateId)
            try {
                const response = await axios.delete(`Candidates/${candidateId}`, {
                    headers: {
                        'XSS-CSRF-TOKEN': csrfToken,
                    }
                });

                setIsSubmitSuccessful("Successfully Deleted Candidate!");

                setCandidateId(null);
                setFile(null);

            } catch (error) {
                console.error('Delete Candidate error:', error.response.data);
            }
    }

    const handleFormData = () => {

        let formData = new FormData();

        formData.append('LastName', lastname);
        formData.append('FirstName', firstname);
        formData.append('Email', email);
        formData.append('Mobile', mobile);
        formData.append('CVFile', file);



        if (degrees.length > 0) {
            let filteredDegrees = degrees.filter(item => item !== "");

            filteredDegrees.forEach((degree, index) => {
                formData.append(`degree[${index}].Name`, degree);
            });
        }
        else
            formData.append('Degree', null);

        return formData;
    }

    const handleSubmit = async (e) => {
        e.preventDefault();

        setLoading(true);

        if (!csrfToken) {
            await fetchCsrfToken();
        }

        if (!validateInput()) {
            setLoading(false);
            return;
        }

        const candidateData = handleFormData();

        await handleLogin(candidateData);

        setLoading(false);
    }

    const addCandidate = async (userData) => {
        try {
            const response = await axios.post('Candidates/add', userData, {
                headers: {
                    'XSS-CSRF-TOKEN': csrfToken,
                }
            });

            setCandidateId(response.data.candidate.id);
            setIsSubmitSuccessful("Submission successful!");

        } catch (error) {
            console.error('Adding Candidate error:', error.response.data);

            setIsSubmitSuccessful(`An error occured, check your information.
                                \nFirst/ Last Name accept maximum 15 English letters each
                                \nEmail must be in a valid format`);
        }
    }

    const editCandidate = async (userData) => {
        try {
            const response = await axios.put(`Candidates/${candidateId}`, userData, {
                headers: {
                    'XSS-CSRF-TOKEN': csrfToken,
                }
            });

            setIsSubmitSuccessful("Successfully Edited Candidate!");

        } catch (error) {
            console.error('Edit Candidate error:', error.response.data);

            setIsSubmitSuccessful(`An error occured, check your information.
                                \nFirst/ Last Name accept maximum 15 English letters each
                                \nEmail must be in a valid format`);
        }
    }

    const handleLogin = async (userData) => {

        if (candidateId)
            await editCandidate(userData);
        else
            await addCandidate(userData);

    };

    const handleDegrees = (index, value) => {
        const updatedDegrees = [...degrees];
        updatedDegrees[index] = value;
        setDegrees(updatedDegrees);

        // add DegreeSelector for the remaining degree values, max 3
        if (value && updatedDegrees.length < 3 && !updatedDegrees.includes("")) {
            setDegrees([...updatedDegrees, ""]);
        }
    };

    const getAvailableOptions = (index) => {

        const selectedValues = degrees.slice(0, index);
        const allOptions = ["B.Sc.", "M.Sc.", "Ph.D."];

        // filter  values that have already been selected
        return allOptions.filter(option => !selectedValues.includes(option));
    };

    return (
        <form onSubmit={handleSubmit} className="candidateForm">
            <div className="title">Candidate Form</div>
            <div className="addressCard"><FontAwesomeIcon icon={faAddressCard} /></div>
            <div className="inputDiv">
                <FontAwesomeIcon icon={faUser} className='inputIcon' />
                <input type="text" id="lastname" placeholder="Lastname" required onChange={(e) => setLastname(sanitizeInput(e.currentTarget.value))} />
                <input type="text" id="firstname" placeholder="Firstname" required onChange={(e) => setFirstname(sanitizeInput(e.currentTarget.value))} />
            </div>
            <div className="inputDiv">
                <FontAwesomeIcon icon={faAt} className='inputIcon' />
                <input type="email" id="email" placeholder="Email" required onChange={(e) => setEmail(sanitizeInput(e.currentTarget.value))} />
            </div>
            <div className="inputDiv">
                <FontAwesomeIcon icon={faPhone} className='inputIcon' />
                <input type="text" id="mobile" placeholder="Mobile" onChange={(e) => setMobile(sanitizeInput(e.currentTarget.value))} />
            </div>
            {degrees.map((degree, index) => (
                <DegreeSelector
                    key={index}
                    index={index}
                    degree={degree}
                    handleChange={handleDegrees}
                    options={getAvailableOptions(index)}
                />
            ))}

            <div className="inputDiv">
                <FontAwesomeIcon icon={faFileLines} className='inputIcon' />
                <label style={{ display: file ? "none" : "block" }} htmlFor="file" >Click to Upload your Curriculum Vitae</label>
                <input style={{ display: file ? "block" : "none" }} type="file" name="file" id="file" onChange={handleFile} accept=".pdf, .doc,.docx, application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document" />
            </div>

            {isFileUploaded}

            <div className="submitBtn">
                <button type="submit" disabled={loading}>
                    {loading ? <>
                        <FontAwesomeIcon icon={faCircleNotch} spin className='loadingCirle' />
                        Submiting...
                    </> : candidateId ? 'Edit Candidate' : 'Submit'}
                </button>
            </div>
            {candidateId &&
                <div className="deleteBtn">
                    <button type="reset" onClick={handleDelete} disabled={loading}>
                        {loading ? <>
                            <FontAwesomeIcon icon={faCircleNotch} spin className='loadingCirle' />
                            Deleting...
                        </> : 'Delete Candidate'}
                    </button>
                </div>
            }
            <div className="success" >
                <p>{isSubmitSuccessful}</p>
            </div>
        </form>
    )
}
